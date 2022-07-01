﻿using StereoKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StereoKitApp
{
    internal abstract class PhobicStimulus
    {
        protected List<Model> models = new List<Model>();
        private Solid _solid;
        private readonly Random _random = new Random();
        private int _level = 0;
        private Model _activeModel;
        private int _maxModelLevel { get => models.Count - 1; }

        // walking
        private bool _roamingOn = true;
        private DateTime _lastWalkingChange = DateTime.Now;
        private bool _isWalking = true;

        // debug
        private Pose _debugWindowPose = new Pose(0.4f, 0, -0.4f, Quat.LookDir(0, 0, 1));
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Setting to turn on/off the model roaming behavior. If roaming, model will move around.
        /// </summary>
        public bool RoamingOn { get => _roamingOn; set => _roamingOn = value; }

        public float Scale { get; set; } = 3;

        /// <summary>
        /// Current level of the model
        /// </summary>
        public int Level
        {
            get { return _level; }
            set
            {
                if (_level < 0)
                    throw new ArgumentOutOfRangeException(nameof(Level), "The Level must be greater than or equal to 0");

                if (_level + 1 > models.Count)
                    throw new IndexOutOfRangeException($"The Level {_level} does not exist. The maximum Level is {models.Count - 1}");

                _level = value;
                _activeModel = models[_level];

                // sync model animation to the current animation state
                syncAnimation();

                // TODO when the model changes, also change the solid's physical shape
                // Maybe this would be best accomplished by encapsulating each model in another object
                // that has a corresponding Solid. In that case, we can init each solid
                // during the InitModels call.
            }
        }

        public PhobicStimulus()
        {
        }

        /// <summary>
        /// Load all the models for the stimulus
        /// </summary>
        protected abstract void InitModels();

        /// <summary>
        /// Call after creating the stimulus to initialize! Only call DURING/AFTER the StereoKit app initialization.
        /// </summary>
        public void Init()
        {
            InitModels();
            Level = 1;

            // The initial position of the object
            _solid = new Solid(V.XYZ(0, 0, -1), Quat.Identity);

            // Use the last model's bounds since it looks the best
            _solid.AddBox(models.Last().Bounds.dimensions * Scale, 1);
        }

        /// <summary>
        /// Step the phobic stimulus.
        /// </summary>
        public void Step()
        {
            // Roaming movement
            if (RoamingOn && _isWalking)
            {
                Pose solidPose = _solid.GetPose();
                Vec3 forwardPose = solidPose.Forward;

                float speed = 0.0015f * Scale;
                Vec3 newPosition = new Vec3();
                newPosition.x = solidPose.position.x + (forwardPose.x * speed);
                newPosition.y = solidPose.position.y;
                newPosition.z = solidPose.position.z + (forwardPose.z * speed);

                // Rotate to walk in a circle
                Quat rotateAroundY = (Matrix.R(0, 0.5f, 0) * solidPose.ToMatrix()).Rotation;

                // Remove wobble
                rotateAroundY.z = 0;
                rotateAroundY.x = 0;

                _solid.Teleport(newPosition, rotateAroundY);
            }

            // Allow user to pick up / move the solid
            Pose sPose = _solid.GetPose();
            if (UI.Handle($"PSTIM_{_id}", ref sPose, _activeModel.Bounds * Scale))
            {
                // We must disable physics when UI handle is in use
                _solid.Enabled = false;
            }
            else
            {
                _solid.Enabled = true;
            }

            // We need the teleport here... not sure why... but it makes it work!
            _solid.Teleport(sPose.position, sPose.orientation);
            _activeModel.Draw(sPose.ToMatrix(Scale));

            // randomly change walking status about every 1/300 steps, with a throttle of 3 seconds
            var timeSinceLastChange = DateTime.Now - _lastWalkingChange;
            if (_random.Next(300) == 1 && timeSinceLastChange.TotalSeconds > 3)
            {
                _isWalking = !_isWalking;
                syncAnimation();
                _lastWalkingChange = DateTime.Now;
            }

            // touching stuff

            if (DebugTools.DEBUG_TOOLS_ON)
            {
                // Draw a UI box to visualize the solid
                Mesh box = Mesh.GenerateCube(_activeModel.Bounds.dimensions);
                box.Draw(Material.UIBox, sPose.ToMatrix(Scale));

                // Window for debug controls
                UI.WindowBegin($"PSTIM_DEBUG_{_id}", ref _debugWindowPose);
                UI.Label($"Level: {Level}");
                UI.SameLine();
                if (UI.ButtonRound($"Down_{_id}", Asset.Instance.IconDown) && _level > 0)
                    Level--;
                UI.SameLine();
                if (UI.ButtonRound($"Up_{_id}", Asset.Instance.IconUp) && _level < _maxModelLevel)
                    Level++;

                UI.Toggle($"IsRoaming_{_id}", ref _roamingOn);

                float scaleVal = Scale;
                UI.Label($"Scale: {scaleVal}");
                UI.HSlider($"Scale_{_id}", ref scaleVal, 0, 10, 0.01f);
                Scale = scaleVal;

                UI.Label($"x: {_solid.GetPose().position.x}");
                UI.Label($"y: {_solid.GetPose().position.y}");
                UI.Label($"z: {_solid.GetPose().position.z}");
                UI.WindowEnd();
            }
        }

        /// <summary>
        /// Set the model's position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void SetPosition(float x, float y, float z)
        {
            var currentRotation = _solid.GetPose().orientation;
            _solid.Teleport(V.XYZ(x, y, z), currentRotation);

            _debugWindowPose.position.x = x;
            _debugWindowPose.position.y = y;
            _debugWindowPose.position.z = z;
        }

        /// <summary>
        /// Set the model's position
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vec3 position)
        {
            SetPosition(position.x, position.y, position.z);
        }

        /// <summary>
        /// Is the hand touching the phobic stimulus this frame?
        /// </summary>
        /// <returns></returns>
        public bool HandIsTouching()
        {
            Hand hand = Input.Hand(Handed.Right);
            Pose fingerTip = hand[FingerId.Index, JointId.Tip].Pose;
            var bounds = new Bounds(_solid.GetPose().position, _activeModel.Bounds.dimensions * Scale);

            return bounds.Contains(fingerTip.position);
        }

        // TODO make this compatible with other PhobicStimulus sub-classes
        private void syncAnimation()
        {
            // last level has animations
            if (_level == _maxModelLevel)
            {
                if (_isWalking)
                    _activeModel.PlayAnim("walk_ani_vor", AnimMode.Loop);
                else
                    _activeModel.PlayAnim("warte_pose", AnimMode.Loop);
            }
        }

    }
}

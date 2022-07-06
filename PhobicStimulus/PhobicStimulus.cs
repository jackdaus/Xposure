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
        private int _modelIntensity = 0;
        private Model _activeModel;
        private int _maxModelLevel { get => models.Count - 1; }
        private bool _isHeld;

        // walking
        private bool _roamingOn = true;
        private DateTime _lastWalkingChange = DateTime.Now;
        private bool _isWalking = true;

        // debug
        private Pose _debugWindowPose = new Pose(0.4f, 0, -0.4f, Quat.LookDir(0, 0, 1));
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Enable or disable the model's roaming behavior. If roaming, model will move around.
        /// </summary>
        public bool RoamingEnabled { get => _roamingOn; set => _roamingOn = value; }

        /// <summary>
        /// Enable or disable the model's physics simulation.
        /// </summary>
        public bool PhysicsEnabled { get; set; }

        public float Scale { get; set; } = 3;

        /// <summary>
        /// Level of realism of the model
        /// </summary>
        public int ModelIntensity
        {
            get { return _modelIntensity; }
            set
            {
                if (_modelIntensity < 0)
                    throw new ArgumentOutOfRangeException(nameof(ModelIntensity), "The ModelIntensity must be greater than or equal to 0");

                if (_modelIntensity + 1 > models.Count)
                    throw new IndexOutOfRangeException($"The ModelIntensity {_modelIntensity} does not exist. The maximum ModelIntensity is {models.Count - 1}");

                _modelIntensity = value;
                _activeModel = models[_modelIntensity];

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
            ModelIntensity = 1;

            // The initial position of the object
            _solid = new Solid(V.XYZ(0, 0, -1), Quat.Identity);

            // Use the last model's bounds since it looks the best. TODO each model should have it's own bounding box, ideally
            _solid.AddBox(models.Last().Bounds.dimensions * Scale, 1);

            // Set physics off initially to prevent the physics system stepping before
            // the client gets a chance to turn off physics!
            _solid.Enabled = false;
            PhysicsEnabled = false;
        }

        /// <summary>
        /// Step the phobic stimulus.
        /// </summary>
        public void Step()
        {
            // Roaming movement
            if (RoamingEnabled && _isWalking)
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
                _isHeld = true;
            }
            else
            {
                // Renable the physics (only if the PhobicStimulus has physics enabled!)
                _solid.Enabled = PhysicsEnabled;
                _isHeld = false;
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

            if (DebugTools.DEBUG_TOOLS_ON)
            {
                // Draw a UI box to visualize the solid
                Mesh box = Mesh.GenerateCube(_activeModel.Bounds.dimensions);
                box.Draw(Material.UIBox, sPose.ToMatrix(Scale));

                // Window for debug controls
                UI.WindowBegin($"PSTIM_DEBUG_{_id}", ref _debugWindowPose);
                UI.Label($"Level: {ModelIntensity}");
                UI.SameLine();
                if (UI.ButtonRound($"Down_{_id}", Asset.Instance.IconDown) && _modelIntensity > 0)
                    ModelIntensity--;
                UI.SameLine();
                if (UI.ButtonRound($"Up_{_id}", Asset.Instance.IconUp) && _modelIntensity < _maxModelLevel)
                    ModelIntensity++;

                UI.Toggle($"IsRoaming_{_id}", ref _roamingOn);

                UI.Label($"Type: {GetType()}");

                float scaleVal = Scale;
                UI.Label($"Scale: {scaleVal}");
                UI.HSlider($"Scale_{_id}", ref scaleVal, 0, 10, 0.01f);
                Scale = scaleVal;
                UI.Label($"Model Position: {_solid.GetPose().position}");
                UI.Label($"Head Position: {Input.Head.position}");
                UI.Label($"Head Forward: {Input.Head.Forward}");

                Vec3 v4 = _solid.GetPose().position - Input.Head.position;
                UI.Label($"v4: {v4}");

                float cosTheta = Vec3.Dot(v4.Normalized, Input.Head.Forward.Normalized);
                UI.Label($"costTheta: {cosTheta}");


                var distance = Vec3.Distance(Input.Head.position, _solid.GetPose().position);
                UI.Label($"Distance to user: {distance}");
                UI.Label($"Looking: {PatientIsLooking()}");


                UI.WindowEnd();
            }
        }

        /// <summary>
        /// Get the model's position
        /// </summary>
        /// <returns></returns>
        public Vec3 GetPosition()
        {
            return _solid.GetPose().position;
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
        public bool PatientIsTouching()
        {
            Bounds modelBounds = new Bounds(_solid.GetPose().position, _activeModel.Bounds.dimensions * Scale);

            // Loop over both hands
            for (int h = 0; h < (int)Handed.Max; h++)
            {
                Hand hand = Input.Hand((Handed)h);
                
                // Only consider tracked hands.
                // Untracked hands can still have a position, even if not visible!
                if (hand.IsTracked)
                {
                    // Loop over all 5 fingers
                    for (int f = 0; f <= (int)FingerId.Little; f++)
                    {
                        Pose fingertip = hand[(FingerId)f, JointId.Tip].Pose;
                        if (modelBounds.Contains(fingertip.position))
                            return true;
                    }
                }
            }

            return false;
        }

        public bool PatientIsLooking()
        {
            // Get the 'angle' between the stimulus and patient's forward direction.
            // If patient is generally facing the stimulus, then return true.
            // TODO would be nice to have eye tracking if supported by the device!
            Vec3 patientToStimulusVec = _solid.GetPose().position - Input.Head.position;
            float cosTheta = Vec3.Dot(patientToStimulusVec.Normalized, Input.Head.Forward.Normalized);
            return cosTheta > 0.55f;
        }

        public bool PatientIsHolding()
        {
            return _isHeld;
        }

        // TODO make this compatible with other PhobicStimulus sub-classes
        private void syncAnimation()
        {
            // last spider level has animations
            if (_modelIntensity == _maxModelLevel)
            {
                if (_isWalking)
                    _activeModel.PlayAnim("walk_ani_vor", AnimMode.Loop);
                else
                    _activeModel.PlayAnim("warte_pose", AnimMode.Loop);
            }
        }

    }
}

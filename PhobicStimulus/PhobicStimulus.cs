using StereoKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal abstract class PhobicStimulus
    {
        protected List<Model> models = new List<Model>();
        private Pose _pose = new Pose(0, 0, 0, Quat.Identity);
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
        private float _debugScale = 1;
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Setting to turn on/off the model roaming behavior. If roaming, model will move around.
        /// </summary>
        public bool RoamingOn { get => _roamingOn; set => _roamingOn = value; }

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

                // toggle animation to sync model to the current animation state
                toggleAnimation();
            }
        }

        public PhobicStimulus()
        {
        }

        /// <summary>
        /// Call after creating the stimulus to initialize all the models! Only call DURING/AFTER the StereoKit app initialization.
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// Step the phobic stimulus.
        /// </summary>
        public void Step()
        {
            var right = _pose.Right;
            var forwardPose = _pose.Forward;

            if (RoamingOn && _isWalking)
            {
                var speed = 0.00045f;
                _pose.position.z += forwardPose.z * speed;
                _pose.position.x += forwardPose.x * speed;
                
                // Rotate 
                _pose = (Matrix.R(0, 0.1f, 0) * _pose.ToMatrix()).Pose;
            }

            UI.Handle($"Spider_{_id}", ref _pose, _activeModel.Bounds * _debugScale);
            _activeModel.Draw(Matrix.S(_debugScale) * _pose.ToMatrix());

            // randomly change walking status about every 1/300 steps, with a throttle of 3 seconds
            var timeSinceLastChange = DateTime.Now - _lastWalkingChange;
            if (_random.Next(300) == 1 && timeSinceLastChange.TotalSeconds > 3)
            {
                _isWalking = !_isWalking;
                toggleAnimation();
                _lastWalkingChange = DateTime.Now;
            }

            if (DebugTools.DEBUG_TOOLS_ON)
            {
                // Window for debug controls
                UI.WindowBegin($"SPIDER_DEBUG_{_id}", ref _debugWindowPose);
                UI.Label($"Level: {Level}");
                UI.SameLine();
                if (UI.ButtonRound($"Down_{_id}", Asset.Instance.IconDown) && _level > 0)
                    Level--;
                UI.SameLine();
                if (UI.ButtonRound($"Up_{_id}", Asset.Instance.IconUp) && _level < _maxModelLevel)
                    Level++;

                UI.Toggle($"IsRoaming_{_id}", ref _roamingOn);

                UI.Label($"Scale: {_debugScale}");
                UI.HSlider($"Scale_{_id}", ref _debugScale, 0, 10, 0.01f);
                UI.Label($"x: {_pose.position.x}");
                UI.Label($"y: {_pose.position.y}");
                UI.Label($"z: {_pose.position.z}");
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
            _pose.position.x = x;
            _pose.position.y = y;
            _pose.position.z = z;

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


        // TODO make this compatible with other PhobicStimulus sub-classes
        private void toggleAnimation()
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

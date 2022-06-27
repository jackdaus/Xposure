using StereoKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal abstract class PhobicStimulus
    {
        protected List<Model> models = new List<Model>();
        private readonly Random _random = new Random();
        private int _level = 0;
        private Model _activeModel;
        private int _maxModelLevel { get => models.Count - 1; }
        

        // walking
        private bool _roamingOn = true;
        private DateTime _lastWalkingChange = DateTime.Now;
        private float _rotateAngle = 0;
        private bool _isWalking = true;
        private Matrix _translateMat = Matrix.T(0.5f, 0, 0);

        // debug
        private Pose _debugWindowPose = new Pose(0.4f, 0, -0.4f, Quat.LookDir(0, 0, 1));
        private float _debugScale = 1;
        private Guid _id = Guid.NewGuid();
        private Pose _cubePose = new Pose(0, 0, 0, Quat.Identity);
        private Model _cube;

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
            // TODO remove cube
            _cube = Model.FromMesh(
                Mesh.GenerateRoundedCube(Vec3.One * 0.1f, 0.02f),
                Default.MaterialUI);
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
            if (RoamingOn && _isWalking) _rotateAngle -= 0.15f;

            var rotateMat = Matrix.R(0, _rotateAngle, 0);
            var trMat = _translateMat * rotateMat;

            if (DebugTools.DEBUG_TOOLS_ON)
                trMat = Matrix.S(_debugScale) * trMat;

            _activeModel.Draw(trMat * _cubePose.ToMatrix());

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
                // cube for easy handling of spider
                UI.Handle($"Cube_{_id}", ref _cubePose, _cube.Bounds);
                _cube.Draw(_cubePose.ToMatrix());

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
                UI.HSlider($"Scale_{_id}", ref _debugScale, 0, 1, 0.01f);
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
            _cubePose.position.x = x;
            _cubePose.position.y = y;
            _cubePose.position.z = z;

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
            _cubePose.position.x = position.x;
            _cubePose.position.y = position.y;
            _cubePose.position.z = position.z;

            _debugWindowPose.position.x = position.x;
            _debugWindowPose.position.y = position.y;
            _debugWindowPose.position.z = position.z;
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

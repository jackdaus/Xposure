using StereoKit;
using System;
using System.Collections.Generic;

namespace StereoKitApp
{
    public class Spider
    {
        public const int MAX_LEVEL = 9;

        Pose _cubePose = new Pose(0, 0, 0, Quat.Identity);
        float _rotateAngle = 0;
        Matrix _translateMat = Matrix.T(0.5f, 0, 0);
        Model _cube;

        Model _activeModel;
        List<Model> _spiderModels = new List<Model>();

        private readonly Random _random = new Random();
        bool _isWalking = true;
        DateTime _lastWalkingChange = DateTime.Now;

        int _level = 0;

        bool _roamingOn = true;

        public bool RoamingOn { get => _roamingOn; set => _roamingOn = value; }

        public int Level
        {
            get { return _level; }
            set 
            {
                if (_level < 0)
                    throw new ArgumentOutOfRangeException(nameof(Level), "The Level must be greater than or equal to 0");

                if (_level + 1 > _spiderModels.Count)
                    throw new IndexOutOfRangeException($"The Level {_level} does not exist. The maximum Level is {_spiderModels.Count - 1}");

                _level = value;
                _activeModel = _spiderModels[_level];

                // toggle animation to sync model to the current animation state
                toggleAnimation();
            }
        }

        // for debug
        Pose _debugWindowPose = new Pose(0.4f, 0, -0.4f, Quat.LookDir(-1, 0, 1));
        float _debugScale = 1;
        Guid _id = Guid.NewGuid();

        public Spider()
        {
            _cube = Model.FromMesh(
                Mesh.GenerateRoundedCube(Vec3.One * 0.1f, 0.02f),
                Default.MaterialUI);

            // initialize level 0 (empty model)
            Model spiderModel = new Model();
            _spiderModels.Add(spiderModel);

            // initialize level 1
            spiderModel = Model.FromMesh(Mesh.GenerateSphere(0.0115f), Default.MaterialUI);
            _spiderModels.Add(spiderModel);

            // initialize level 2
            spiderModel = Model.FromMesh(Mesh.GenerateSphere(0.0115f), Default.MaterialUI);
            spiderModel.RootNode.AddChild("abs", Matrix.T(0, 0, -0.0115f), Mesh.GenerateSphere(0.0184f), Default.MaterialUI);
            _spiderModels.Add(spiderModel);

            // initialize level 3
            spiderModel = Asset.Instance.SpiderModelA.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.045f);
            _spiderModels.Add(spiderModel);

            // initialize level 4
            spiderModel = Asset.Instance.SpiderModelB.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.06f);
            _spiderModels.Add(spiderModel);

            // initialize level 5
            spiderModel = Asset.Instance.SpiderModelC.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.075f);
            _spiderModels.Add(spiderModel);

            // initialize level 6
            spiderModel = Asset.Instance.SpiderModelD.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.075f);
            _spiderModels.Add(spiderModel);

            // initialize level 7
            spiderModel = Asset.Instance.SpiderModelE.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.075f);
            _spiderModels.Add(spiderModel);

            // initialize level 8
            spiderModel = Asset.Instance.SpiderModelF.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.075f);
            _spiderModels.Add(spiderModel);

            // initialize level 9
            spiderModel = Asset.Instance.SpiderModelG.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.0012f);
            spiderModel.PlayAnim("walk_ani_vor", AnimMode.Loop);
            _spiderModels.Add(spiderModel);

            _activeModel = _spiderModels[_level];
        }

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
                if (UI.ButtonRound($"Up_{_id}", Asset.Instance.IconUp) && _level < MAX_LEVEL)
                    Level++;

                UI.Toggle($"IsRoaming_{_id}", ref _roamingOn);

                UI.Label($"Scale: {_debugScale}");
                UI.HSlider($"Scale_{_id}", ref _debugScale, 0, 1, 0.01f);
                UI.WindowEnd();
            }
        }

        public void SetPosition(float x, float y, float z)
        {
            _cubePose.position.x = x;
            _cubePose.position.y = y;
            _cubePose.position.z = z;

            _debugWindowPose.position.x = x;
            _debugWindowPose.position.y = y;
            _debugWindowPose.position.z = z;
        }

        private void toggleAnimation()
        {
            // last level has animations
            if (_level == MAX_LEVEL)
            {
                if (_isWalking)
                    _activeModel.PlayAnim("walk_ani_vor", AnimMode.Loop);
                else
                    _activeModel.PlayAnim("warte_pose", AnimMode.Loop);
            }
        }
    }
}

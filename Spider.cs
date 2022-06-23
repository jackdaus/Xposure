using StereoKit;
using System;
using System.Collections.Generic;

namespace StereoKitApp
{
    public class Spider
    {
        //public const int MAX_LEVEL = 9;
        public const int MAX_LEVEL = 7;

        Pose cubePose = new Pose(0, 0, 0, Quat.Identity);
        float rotateAngle = 0;
        Matrix translateMat = Matrix.T(0.5f, 0, 0);
        Model cube;

        Model activeModel;
        List<Model> spiderModels = new List<Model>();

        private readonly Random _random = new Random();
        bool isWalking = true;
        DateTime lastWalkingChange = DateTime.Now;

        int _level = 0;
        public int Level
        {
            get { return _level; }
            set 
            {
                if (_level < 0)
                    throw new ArgumentOutOfRangeException(nameof(Level), "The Level must be greater than or equal to 0");

                if (_level + 1 > spiderModels.Count)
                    throw new IndexOutOfRangeException($"The Level {_level} does not exist. The maximum Level is {spiderModels.Count - 1}");

                _level = value;
                activeModel = spiderModels[_level];

                // toggle animation to sync model to the current animation state
                toggleAnimation();
            }
        }

        public Spider()
        {
            cube = Model.FromMesh(
                Mesh.GenerateRoundedCube(Vec3.One * 0.1f, 0.02f),
                Default.MaterialUI);

            // initialize level 0 (empty model)
            Model spiderModel = new Model();
            spiderModels.Add(spiderModel);

            // initialize level 1
            spiderModel = Model.FromMesh(Mesh.GenerateSphere(0.05f), Default.MaterialUI);
            spiderModels.Add(spiderModel);

            // initialize level 2
            spiderModel = Model.FromMesh(Mesh.GenerateSphere(0.05f), Default.MaterialUI);
            spiderModel.RootNode.AddChild("test", Matrix.T(0, 0, -0.05f), Mesh.GenerateSphere(0.08f), Default.MaterialUI);
            spiderModels.Add(spiderModel);

            // initialize level 3
            spiderModel = Asset.Instance.SpiderModelA;
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.4f);
            spiderModels.Add(spiderModel);

            // initialize level 4
            spiderModel = Asset.Instance.SpiderModelB;
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.4f);
            spiderModels.Add(spiderModel);

            // TODO fix models
            //// initialize level 5
            //spiderModel = Asset.Instance.SpiderModelC;
            //spiderModel.RootNode.ModelTransform *= Matrix.S(0.4f);
            //spiderModels.Add(spiderModel);

            //// initialize level 6
            //spiderModel = Asset.Instance.SpiderModelD;
            //spiderModel.RootNode.ModelTransform *= Matrix.S(0.4f);
            //spiderModels.Add(spiderModel);

            // initialize level 7
            spiderModel = Asset.Instance.SpiderModelE;
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.4f);
            spiderModels.Add(spiderModel);

            // initialize level 8
            spiderModel = Asset.Instance.SpiderModelF;
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.4f);
            spiderModels.Add(spiderModel);

            // initialize level 9
            spiderModel = Asset.Instance.SpiderModelG.Copy(); // call Copy() so we can play different animations for each spider instance
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.003f);
            spiderModel.PlayAnim("walk_ani_vor", AnimMode.Loop);
            spiderModels.Add(spiderModel);

            activeModel = spiderModels[_level];
        }

        public void Step()
        {
            if (isWalking) rotateAngle -= 0.15f;

            var rotateMat = Matrix.R(0, rotateAngle, 0);
            var trMat = translateMat * rotateMat;

            // Create a UI handle cube where the model will be rooted. Useful for debugging
            //UI.Handle("Cube", ref cubePose, cube.Bounds);
            //cube.Draw(cubePose.ToMatrix());

            activeModel.Draw(trMat * cubePose.ToMatrix());

            // randomly change walking status about every 1/300 steps, with a throttle of 3 seconds
            var timeSinceLastChange = DateTime.Now - lastWalkingChange;
            if (_random.Next(300) == 1 && timeSinceLastChange.TotalSeconds > 3)
            {

                isWalking = !isWalking;
                toggleAnimation();
                lastWalkingChange = DateTime.Now;
            }
        }

        public void SetPosition(float x, float y, float z)
        {
            cubePose.position.x = x;
            cubePose.position.y = y;
            cubePose.position.z = z;
        }

        private void toggleAnimation()
        {
            // last level has animations
            if (_level == MAX_LEVEL)
            {
                if (isWalking)
                    activeModel.PlayAnim("walk_ani_vor", AnimMode.Loop);
                else
                    activeModel.PlayAnim("warte_pose", AnimMode.Loop);
            }
        }
    }
}

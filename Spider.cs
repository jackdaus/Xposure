using StereoKit;
using System;
using System.Collections.Generic;

namespace StereoKitApp
{
    public class Spider
    {
        // TODO use floor height for y coord
        Pose cubePose = new Pose(0, -1.45f, -1f, Quat.Identity);
        float rotateAngle = 0;
        Matrix translateMat = Matrix.T(0.5f, 0, 0);
        Model cube;

        int level = 0;
        Model activeModel;
        List<Model> spiderModels = new List<Model>();

        private readonly Random _random = new Random();
        bool isWalking = true;
        DateTime lastWalkingChange;

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
            spiderModel = Asset.Instance.SpiderModel1.Copy();           // call Copy() so we can play different animations for each spider instance
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.003f);    // Scale the spider
            spiderModel.PlayAnim("walk_ani_vor", AnimMode.Loop);
            spiderModels.Add(spiderModel);
            lastWalkingChange = DateTime.Now;

            level = 3;
            activeModel = spiderModels[level];
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
                // change animation for level 3
                if (level == 3)
                {
                    if (isWalking)
                        activeModel.PlayAnim("warte_pose", AnimMode.Loop);
                    else
                        activeModel.PlayAnim("walk_ani_vor", AnimMode.Loop);
                }

                isWalking = !isWalking;
                lastWalkingChange = DateTime.Now;
            }
        }

        /// <summary>
        /// Get the current level of this spider
        /// </summary>
        /// <returns></returns>
        public int getCurrentLevel()
        {
            return level;
        }

        /// <summary>
        /// Get the maximum level
        /// </summary>
        /// <returns></returns>
        public int getMaxLevel()
        {
            return spiderModels.Count - 1;
        }

        /// <summary>
        /// Set the current level of intensity for the spider
        /// </summary>
        /// <param name="level"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void setLevel(int level)
        {
            if (level < 0)
                throw new ArgumentOutOfRangeException(nameof(level), "The level must be greater than or equal to 0");

            if (level + 1 > spiderModels.Count)
                throw new IndexOutOfRangeException($"The level {level} does not exist. The maximum level is {spiderModels.Count - 1}");

            this.level = level;
            activeModel = spiderModels[level];
        }

        public void setPosition(float x, float y, float z)
        {
            cubePose.position.x = x;
            cubePose.position.y = y;
            cubePose.position.z = z;
        }
    }
}

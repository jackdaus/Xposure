using StereoKit;

namespace StereoKitApp
{
    public class Spider
    {
        Pose cubePose = new Pose(0, -1.5f, -1.5f, Quat.Identity);
        float rotateAngle = 0;
        Matrix translateMat = Matrix.T(0.5f, 0, 0);
        Model cube;
        Model spiderModel;

        public Spider()
        {
            cube = Model.FromMesh(
                Mesh.GenerateRoundedCube(Vec3.One * 0.1f, 0.02f),
                Default.MaterialUI);

            spiderModel = Model.FromFile("spider/scene.gltf");
            // Scale the spider
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.003f);

            spiderModel.PlayAnim("walk_ani_vor", AnimMode.Loop);
        }

        public void Step()
        {
            rotateAngle -= 0.15f;
            var rotateMat = Matrix.R(0, rotateAngle, 0);
            var trMat = translateMat * rotateMat;

            // Create a UI handle cube where the model will be rooted. Useful for debugging
            UI.Handle("Cube", ref cubePose, cube.Bounds);
            cube.Draw(cubePose.ToMatrix());
            
            spiderModel.Draw(trMat * cubePose.ToMatrix());
        }
    }
}

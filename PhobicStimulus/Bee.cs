using StereoKit;

namespace StereoKitApp
{
    internal class Bee : PhobicStimulus
    {
        public Bee()
        {
        }

        protected override void InitModels()
        {
            // initialize level 0 (empty model)
            Model beeModel = new Model();
            models.Add(beeModel);

            // initialize level 1
            beeModel = Model.FromMesh(Mesh.GenerateSphere(0.0115f), Default.MaterialUI);
            models.Add(beeModel);

            // initialize level 2
            beeModel = Model.FromMesh(Mesh.GenerateSphere(0.0115f), Default.MaterialUI);
            beeModel.RootNode.AddChild("abs", Matrix.T(0, 0, -0.0115f), Mesh.GenerateSphere(0.0184f), Default.MaterialUI);
            beeModel.RootNode.ModelTransform *= Matrix.R(0, 180, 0);
            models.Add(beeModel);

            // initialize level 3
            beeModel = Asset.Instance.SpiderModelA.Copy();
            beeModel.RootNode.ModelTransform *= Matrix.S(0.045f);
            beeModel.RootNode.ModelTransform *= Matrix.R(0, 180, 0);
            models.Add(beeModel);

            // initialize level 4
            beeModel = Asset.Instance.Bee.Copy();
            beeModel.RootNode.ModelTransform *= Matrix.S(0.05f);
            models.Add(beeModel);

            // initialize level 4 animated! but texture doesn't load :(
            //beeModel = Asset.Instance.Bee.Copy();
            //beeModel.RootNode.ModelTransform *= Matrix.S(0.001f); S
            //beeModel.PlayAnim("_bee_hover", AnimMode.Loop);
            //models.Add(beeModel);
        }
    }
}

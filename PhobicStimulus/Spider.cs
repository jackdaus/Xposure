using StereoKit;

namespace StereoKitApp
{
    internal class Spider : PhobicStimulus
    {
        public Spider()
        {
        }

        protected override void InitModels()
        {
            // initialize level 0 (empty model)
            Model spiderModel = new Model();
            models.Add(spiderModel);

            // initialize level 1
            spiderModel = Model.FromMesh(Mesh.GenerateSphere(0.0115f), Default.MaterialUI);
            models.Add(spiderModel);

            // initialize level 2
            spiderModel = Model.FromMesh(Mesh.GenerateSphere(0.0115f), Default.MaterialUI);
            spiderModel.RootNode.AddChild("abs", Matrix.T(0, 0, -0.0115f), Mesh.GenerateSphere(0.0184f), Default.MaterialUI);
            spiderModel.RootNode.ModelTransform *= Matrix.R(0, 180, 0);
            models.Add(spiderModel);

            // initialize level 3
            spiderModel = Asset.Instance.SpiderModelA.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.045f);
            spiderModel.RootNode.ModelTransform *= Matrix.R(0, 180, 0);
            models.Add(spiderModel);

            // initialize level 4
            spiderModel = Asset.Instance.SpiderModelB.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.06f);
            spiderModel.RootNode.ModelTransform *= Matrix.R(0, 180, 0);
            models.Add(spiderModel);

            // initialize level 5
            spiderModel = Asset.Instance.SpiderModelC.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.075f);
            spiderModel.RootNode.ModelTransform *= Matrix.R(0, 180, 0);
            models.Add(spiderModel);

            // initialize level 6
            spiderModel = Asset.Instance.SpiderModelD.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.075f);
            spiderModel.RootNode.ModelTransform *= Matrix.R(0, 180, 0);
            models.Add(spiderModel);

            // initialize level 7
            spiderModel = Asset.Instance.SpiderModelE.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.075f);
            spiderModel.RootNode.ModelTransform *= Matrix.R(0, 180, 0);
            models.Add(spiderModel);

            // initialize level 8
            spiderModel = Asset.Instance.SpiderModelF.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.075f);
            spiderModel.RootNode.ModelTransform *= Matrix.R(0, 180, 0);
            models.Add(spiderModel);

            // initialize level 9
            spiderModel = Asset.Instance.SpiderModelG.Copy();
            spiderModel.RootNode.ModelTransform *= Matrix.S(0.0012f);
            spiderModel.RootNode.ModelTransform *= Matrix.R(0, 180, 0);
            spiderModel.PlayAnim("walk_ani_vor", AnimMode.Loop);
            models.Add(spiderModel);
        }
    }
}

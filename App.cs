using StereoKit;

namespace StereoKitApp
{
	public class App
	{
		public SKSettings Settings => new SKSettings { 
			appName           = "StereoKit Template",
			assetsFolder      = "Assets",
			displayPreference = DisplayMode.MixedReality
		};

		Pose  cubePose = new Pose(0, -1.5f, -0.5f, Quat.Identity);
		Model cube;
		Matrix   floorTransform = Matrix.TS(new Vec3(0, -1.5f, 0), new Vec3(30, 0.1f, 30));
		Material floorMaterial;

		Model spider;
		// Shrink the spider
		Matrix spiderTransform = Matrix.S(0.01f);

		public void Init()
		{
			// Create assets used by the app
			cube = Model.FromMesh(
				Mesh.GenerateRoundedCube(Vec3.One * 0.1f, 0.02f),
				Default.MaterialUI);

			floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
			floorMaterial.Transparency = Transparency.Blend;

			spider = Model.FromFile("spider/scene.gltf");
			spider.RootNode.ModelTransform *= spiderTransform;


			// log the animations available
			foreach (Anim anim in spider.Anims)
				Log.Info($"Animation: {anim.Name} {anim.Duration}s");

			spider.PlayAnim("walk_ani_vor", AnimMode.Loop);
		}

		public void Step()
		{
			if (SK.System.displayType == Display.Opaque)
				Default.MeshCube.Draw(floorMaterial, floorTransform);

			UI.Handle("Cube", ref cubePose, cube.Bounds);
			cube.Draw(cubePose.ToMatrix());
			spider.Draw(cubePose.ToMatrix());
		}
	}
}
using StereoKit;
using System;

namespace StereoKitApp
{
	public class App
	{
		public SKSettings Settings => new SKSettings { 
			appName           = "StereoKit Template",
			assetsFolder      = "Assets",
			displayPreference = DisplayMode.MixedReality
		};

		Matrix floorTransform;
		Material floorMaterial;
		LevelManager lvlManager = new LevelManager(new Pose(0, 0, 0, Quat.LookDir(0, 0, 0)), new Vec2(10, 0) * U.cm);
		Spider mySpider;
        Spider mySpider2;

		Pose menuPose = new Pose(0.4f, 0, -0.4f, Quat.LookDir(-1, 0, 1));
		int currentLevel = 0;
		const int MAX_LEVEL = Spider.MAX_LEVEL;

		public void Init()
		{
			float floorHeight = World.HasBounds ? World.BoundsPose.position.y : -1.5f;

			// Create assets used by the app
			floorTransform = Matrix.TS(new Vec3(0, floorHeight, 0), new Vec3(30, 0.1f, 30));
			floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
			floorMaterial.Transparency = Transparency.Blend;

			if (World.HasBounds)
            {
                Log.Info("Has bounds!");
                Log.Info($"World pose: ({World.BoundsPose.position.x}, {World.BoundsPose.position.y}, {World.BoundsPose.position.z})");
			}
        }

		public void Step()
		{
			if (SK.System.displayType == Display.Opaque)
				Default.MeshCube.Draw(floorMaterial, floorTransform);

			lvlManager.Step();

			drawGlobalCoordinates();
		}

		/// <summary>
		/// Draw the unit coordinates at the origin. Useful for understanding your current orientation while debugging.
		/// </summary>
		private void drawGlobalCoordinates()
		{
			Lines.Add(
				new Vec3(0, 0, 0),
				new Vec3(1, 0, 0),
				Color.Black,
				Color.HSV(0, 1, 1),
				1 * U.cm);

			Lines.Add(
				new Vec3(0, 0, 0),
				new Vec3(0, 1, 0),
				Color.Black,
				Color.HSV(1f / 3f, 1, 1),
				1 * U.cm);

			Lines.Add(
				new Vec3(0, 0, 0),
				new Vec3(0, 0, 1),
				Color.Black,
				Color.HSV(2f / 3f, 1, 1),
				1 * U.cm);
		}

	}
}
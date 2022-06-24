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
		LevelManager lvlManager;

		public void Init()
		{
			// Create assets used by the app
			floorTransform = Matrix.TS(new Vec3(0, Util.FloorHeight, 0), new Vec3(30, 0.1f, 30));
			floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
			floorMaterial.Transparency = Transparency.Blend;

			lvlManager = new LevelManager();
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
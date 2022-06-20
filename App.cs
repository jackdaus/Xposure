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

		Matrix   floorTransform = Matrix.TS(new Vec3(0, -1.5f, 0), new Vec3(30, 0.1f, 30));
		Material floorMaterial;

		Spider mySpider;

		public void Init()
		{
			// Create assets used by the app
			floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
			floorMaterial.Transparency = Transparency.Blend;

			mySpider = new Spider();
		}

		public void Step()
		{
			if (SK.System.displayType == Display.Opaque)
				Default.MeshCube.Draw(floorMaterial, floorTransform);

			drawGlobalCoordinates();
			mySpider.Step();
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
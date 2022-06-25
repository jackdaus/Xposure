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

			// Change up the color for fun
			UI.ColorScheme = new Color(0.5f, 0.3f, 0.7f);
		}

		public void Step()
		{
			if (SK.System.displayType == Display.Opaque)
				Default.MeshCube.Draw(floorMaterial, floorTransform);

			lvlManager.Step();

			DebugTools.DrawGlobalCoordinates();
		}
	}
}
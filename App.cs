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


		// for debug
        Spider debugSpider1;
        Spider debugSpider2;

        public void Init()
		{
			// Create assets used by the app
			floorTransform = Matrix.TS(new Vec3(0, Util.FloorHeight, 0), new Vec3(30, 0.1f, 30));
			floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
			floorMaterial.Transparency = Transparency.Blend;

			lvlManager = new LevelManager();
			Log.Subscribe(DebugTools.OnLog);

			if (DebugTools.DEBUG_TOOLS_ON)
            {
				debugSpider1 = new Spider();
				debugSpider1.SetPosition(0.1f, -0.1f, -0.3f);
				debugSpider1.Level = 9;

				debugSpider2 = new Spider();
				debugSpider2.SetPosition(-0.1f, -0.1f, -0.3f);
				debugSpider2.Level = 5;
            }

			// Change up the color for fun
			UI.ColorScheme = new Color(0.5f, 0.3f, 0.7f);
		}

		public void Step()
		{
			if (SK.System.displayType == Display.Opaque)
				Default.MeshCube.Draw(floorMaterial, floorTransform);

			lvlManager.Step();
			DebugTools.LogWindow();

			//DebugTools.DrawGlobalCoordinates();

			if (DebugTools.DEBUG_TOOLS_ON)
            {
				debugSpider1.Step();
				debugSpider2.Step();
            }
		}
	}
}
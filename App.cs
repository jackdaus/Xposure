using StereoKit;
using System;

namespace StereoKitApp
{
	public class App
	{
		public SKSettings Settings => new SKSettings { 
			appName           = "XposuRe",
			assetsFolder      = "Assets",
			displayPreference = DisplayMode.MixedReality
		};

		Matrix floorTransform;
		Material floorMaterial;
		Solid floorSolid;
		LevelManager lvlManager;
		public static PassthroughFBExt passthrough;

		public void Init()
		{
			DebugTools.Init();
			//DebugTools.DEBUG_TOOLS_ON = true;

			// Create assets used by the app
			Vec3 floorPosition = new Vec3(0, Util.FloorHeight, 0);
			floorTransform = Matrix.TS(floorPosition, new Vec3(30, 0.1f, 30));
			floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
			floorMaterial.Transparency = Transparency.Blend;

			floorSolid = new Solid(floorPosition, Quat.Identity, SolidType.Immovable);
			floorSolid.AddBox(new Vec3(20, 1000, 20), offset: new Vec3(0, -500, 0));

			// Prevent hands from interacting with the physics system.
			// The hands don't play very nicely with the other solids
			Input.HandSolid(Handed.Max, false);

			lvlManager = new LevelManager();
		}


		// temp
		Pose windowPose = new Pose(0.5f, 0, -0.5f, Quat.LookDir(-1, 0, 1));

		public void Step()
		{
			DebugTools.Step();

			// Only draw floor when using VR headset with no AR passthrough
            if (SK.System.displayType == Display.Opaque && !App.passthrough.EnabledPassthrough)
                Default.MeshCube.Draw(floorMaterial, floorTransform);

            lvlManager.Step();

            // passthrough
            UI.WindowBegin("Passthrough Settings", ref windowPose);
            bool toggle = App.passthrough.EnabledPassthrough;
            UI.Label(App.passthrough.Available
                ? "Passthrough EXT available!"
                : "No passthrough EXT available :(");
            if (UI.Toggle("Passthrough", ref toggle))
                App.passthrough.EnabledPassthrough = toggle;
            UI.WindowEnd();
        }
	}
}
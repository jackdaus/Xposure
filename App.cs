using StereoKit;
using System;

namespace StereoKitApp
{
	public class App
	{
		public SKSettings Settings => new SKSettings { 
			appName           = "Xposure Therapy",
			assetsFolder      = "Assets",
			displayPreference = DisplayMode.MixedReality
		};
		public static PassthroughFBExt Passthrough;
		
		// Quick fix for different font files
		public static bool IsAndroid;
		
		internal static Credits CreditsStepper				= SK.AddStepper(new Credits());
		internal static LevelManager LevelManagerStepper	= SK.AddStepper(new LevelManager());
		internal static DebugTools DebugToolsStepper		= SK.AddStepper(new DebugTools());

		Matrix floorTransform;
		Material floorMaterial;
		Solid floorSolid;

		public void Init()
		{
			// Create assets used by the app
			Vec3 floorPosition = new Vec3(0, Util.FloorHeight, 0);
			floorTransform = Matrix.TS(floorPosition, new Vec3(30, 0f, 30));
			floorMaterial = new Material(Shader.FromFile("floor.hlsl"));
			floorMaterial.Transparency = Transparency.Blend;

			// Create a floor for our physics system
			floorSolid = new Solid(floorPosition, Quat.Identity, SolidType.Immovable);
			floorSolid.AddBox(new Vec3(20, 1000, 20), offset: new Vec3(0, -500, 0));

			// Prevent hands from interacting with the physics system.
			// The hands don't play very nicely with the other solids
			Input.HandSolid(Handed.Max, false);
		}

		public void Step()
		{
			// Only draw floor when using VR headset with no AR passthrough
            if (SK.System.displayType == Display.Opaque && !App.Passthrough.EnabledPassthrough)
                Default.MeshCube.Draw(floorMaterial, floorTransform);
        }
	}
}
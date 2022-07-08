using StereoKit;
using StereoKit.Framework;
using System.Collections.Generic;

namespace StereoKitApp
{
    public class DebugTools : IStepper
    {
		// These must be readonly since they are tied to the Init event
		private readonly bool DEBUG_SPIDERS_ON	= false;
		private readonly bool DEBUG_BEES_ON		= false;

		private Pose logPose = new Pose(1, 0, -0.5f, Quat.LookDir(-1, 0, 1));
		private List<string> logList = new List<string>();
		private string logText = "";

		private static Spider debugSpider1	= new Spider();
		private static Bee debugBee1		= new Bee();

		/// <summary>
		/// Enable the debug tools by changing this
		/// </summary>
		public bool Enabled { get; } = false;

		public bool Initialize()
		{
			Log.Subscribe(onLog);

			if (DEBUG_SPIDERS_ON)
			{
				debugSpider1.Init();
				debugSpider1.PhysicsEnabled = false;
				debugSpider1.SetPosition(0f, 0, -0.5f);
				debugSpider1.ModelIntensity = 1;
				debugSpider1.RoamingMode = Roaming.Walk;
			}

			if (DEBUG_BEES_ON)
			{
				Vec3 position = new Vec3(0, 0, -1f);
				debugBee1.Init();
				debugBee1.SetPosition(position);
				debugBee1.ModelIntensity = 4;
				debugBee1.SoundEnabled = true;
				debugBee1.RoamingMode = Roaming.Fly;
			}

			return true;
		}

		public void Step()
        {
			if (Enabled)
			{
				logWindow();

				// Add label for finger tip position
				Hand hand = Input.Hand(Handed.Right);
				Pose fingerTip = hand[FingerId.Index, JointId.Tip].Pose;
				Text.Add($"{fingerTip.position}", Matrix.T(V.XYZ(0, 0.1f, 0)) * Matrix.TR(fingerTip.position, Quat.LookDir(0, 0, 1)));
			}

			if (DEBUG_SPIDERS_ON)
            {
				debugSpider1.Step();
            }

			if (DEBUG_BEES_ON)
			{
				debugBee1.Step();
			}
		}

		public void Shutdown()
		{
		}

		/// <summary>
		/// Draw the unit coordinates at the origin. Useful for understanding your current orientation while debugging.
		/// </summary>
		public void DrawGlobalCoordinates()
		{
			if (Enabled)
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

		private void onLog(LogLevel level, string text)
		{
			if (logList.Count > 15)
				logList.RemoveAt(logList.Count - 1);
			logList.Insert(0, text.Length < 100 ? text : text.Substring(0, 100) + "...\n");

			logText = "";
			for (int i = 0; i < logList.Count; i++)
				logText += logList[i];
		}

		private void logWindow()
		{
			UI.WindowBegin("Log", ref logPose, new Vec2(40, 0) * U.cm);
			UI.Text(logText);
			UI.HSeparator();
			float fps = 1 / Time.Elapsedf;
			UI.Text($"FPS: {fps}");
			UI.Text($"World Has Bounds: {World.HasBounds}");
			UI.Text($"World Bounds Height: {World.BoundsPose.position.y}");
			UI.Text($"Passthrough Available: {App.Passthrough.Available}");
			UI.WindowEnd();
		}
    }
}

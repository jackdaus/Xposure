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

		private Pose _logPose = new Pose(1, 0, -0.5f, Quat.LookDir(-1, 0, 1));
		private List<string> _logList = new List<string>();
		private string _logText = "";

		private static Spider _spider	= new Spider();
		private static Bee _bee			= new Bee();

		/// <summary>
		/// Enable the debug tools by changing this
		/// </summary>
		public bool Enabled { get; } = false;

		public bool Initialize()
		{
			Log.Subscribe(onLog);

			if (DEBUG_SPIDERS_ON)
			{
				_spider.Init();
				_spider.PhysicsEnabled = true;
				_spider.SetPosition(0f, 0, -0.5f);
				_spider.ModelIntensity = 9;
				_spider.RoamingMode = Roaming.Walk;
				_spider.AnimationEnabled = true;
			}

			if (DEBUG_BEES_ON)
			{
				Vec3 position = new Vec3(0, 0, -1f);
				_bee.Init();
				_bee.SetPosition(position);
				_bee.ModelIntensity = 4;
				_bee.SoundEnabled = true;
				_bee.RoamingMode = Roaming.Fly;
				_bee.AnimationEnabled = true;
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
				_spider.Step();
            }

			if (DEBUG_BEES_ON)
			{
				_bee.Step();
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
			if (_logList.Count > 15)
				_logList.RemoveAt(_logList.Count - 1);
			_logList.Insert(0, text.Length < 100 ? text : text.Substring(0, 100) + "...\n");

			_logText = "";
			for (int i = 0; i < _logList.Count; i++)
				_logText += _logList[i];
		}

		private void logWindow()
		{
			UI.WindowBegin("Log", ref _logPose, new Vec2(40, 0) * U.cm);
			UI.Text(_logText);
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

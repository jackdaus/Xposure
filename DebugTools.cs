using StereoKit;
using System.Collections.Generic;

namespace StereoKitApp
{
    public static class DebugTools
    {
		/// <summary>
		/// Conveiently toggle the debug tools on/off here!
		/// </summary>
		public static bool DEBUG_TOOLS_ON = false;
		public readonly static bool DEBUG_SPIDERS_ON = false;

		private static Pose logPose = new Pose(1, 0, -0.5f, Quat.LookDir(-1, 0, 1));
		private static List<string> logList = new List<string>();
		private static string logText = "";

		private static Spider debugSpider1 = new Spider();
		private static Spider debugSpider2 = new Spider();

		public static void Init()
        {
			Log.Subscribe(onLog);

			if (DEBUG_SPIDERS_ON)
            {
				debugSpider1.Init();
				debugSpider1.SetPosition(0f, 0, -0.5f);
				debugSpider1.Level = 9;
				debugSpider1.RoamingOn = true;

				debugSpider2.Init();
				debugSpider1.SetPosition(0f, 0, -1f);
				debugSpider1.Level = 9;
				debugSpider1.RoamingOn = true;
			}
		}

		public static void Step()
        {
			logWindow();

			if (DEBUG_SPIDERS_ON)
            {
				debugSpider1.Step();
                debugSpider2.Step();
            }
		}

		/// <summary>
		/// Draw the unit coordinates at the origin. Useful for understanding your current orientation while debugging.
		/// </summary>
		public static void DrawGlobalCoordinates()
		{
			if (DEBUG_TOOLS_ON)
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

		private static void onLog(LogLevel level, string text)
		{
			if (logList.Count > 15)
				logList.RemoveAt(logList.Count - 1);
			logList.Insert(0, text.Length < 100 ? text : text.Substring(0, 100) + "...\n");

			logText = "";
			for (int i = 0; i < logList.Count; i++)
				logText += logList[i];
		}

		private static void logWindow()
		{
			if (DEBUG_TOOLS_ON)
            {
				UI.WindowBegin("Log", ref logPose, new Vec2(40, 0) * U.cm);
				UI.Text(logText);
				UI.HSeparator();
				float fps = 1 / Time.Elapsedf;
				UI.Text(fps.ToString());
				UI.WindowEnd();
            }
		}
	}
}

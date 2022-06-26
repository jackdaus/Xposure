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

		static Pose logPose = new Pose(0, -0.1f, 0.5f, Quat.LookDir(Vec3.Forward));
		static List<string> logList = new List<string>();
		static string logText = "";
		public static void OnLog(LogLevel level, string text)
		{
			if (logList.Count > 15)
				logList.RemoveAt(logList.Count - 1);
			logList.Insert(0, text.Length < 100 ? text : text.Substring(0, 100) + "...\n");

			logText = "";
			for (int i = 0; i < logList.Count; i++)
				logText += logList[i];
		}
		public static void LogWindow()
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

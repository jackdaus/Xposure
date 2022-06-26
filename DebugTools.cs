using StereoKit;

namespace StereoKitApp
{
    public static class DebugTools
    {
		/// <summary>
		/// Conveiently toggle the debug tools on/off here!
		/// </summary>
		public static bool DEBUG_TOOLS_ON = true;

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
	}
}

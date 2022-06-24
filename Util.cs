using System;
using System.Collections.Generic;
using System.Text;
using StereoKit;

namespace StereoKitApp
{
    /// <summary>
    /// Common helper methods
    /// </summary>
    static class Util
    {
        /// <summary>
        /// Get the floor height. Uses or World.BoundsPose or else falls back to -1.5f
        /// </summary>
        public static float FloorHeight { get => World.HasBounds? World.BoundsPose.position.y : -1.5f; }
    }
}

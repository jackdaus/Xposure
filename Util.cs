﻿using System;
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
        public static float FloorHeight { get => (World.HasBounds && World.BoundsPose.position.y != 0)? World.BoundsPose.position.y : -1.5f; }

        public static class Colors
        {
            public static readonly Color Red      = new Color(1, 0, 0);
            public static readonly Color Green    = new Color(0, 1, 0);
            public static readonly Color Blue     = new Color(0, 0, 1);
        }

    }
}

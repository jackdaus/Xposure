using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal struct TouchObjective : IObjective
    {
        public int TouchCount { get; set; }

        public TouchObjective(int count)
        {
            TouchCount = count;
        }

        public string Description(SessionHistory history)
        {
            var details = $"Touch the object {TouchCount} {(TouchCount == 1 ? "time" : "times")}";

            if (!IsCompleted(history))
            {
                var remainingTouches = Math.Max(0, TouchCount - history.GetCurrentLevelTouchTimePeriods().Count);
                details += $" ({remainingTouches} remaining)";
            }

            return details;
        }

        public bool IsCompleted(SessionHistory history)
        {
            return history.GetCurrentLevelTouchTimePeriods().Count >= TouchCount;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal struct TouchObjective : IObjective
    {
        public string Description { get => $"Touch the object {TouchCount} times"; }
        public int TouchCount { get; set; }

        public TouchObjective(int count)
        {
            TouchCount = count;
        }

        public bool IsCompleted(SessionHistory history)
        {
            return history.GetCurrentLevelTouchTimePeriods().Count >= TouchCount;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp.Objective
{
    internal class LookObjective : IObjective
    {
        public string Description { get => $"Look at the object {Seconds} seconds"; }
        public int Seconds { get; set; }

        public LookObjective(int seconds)
        {
            Seconds = seconds;
        }

        public bool IsCompleted(SessionHistory history)
        {
            int totalTime = 0;
            foreach (TimePeriod lookTime in history.GetCurrentLevelLookTimePeriods()) {
                totalTime += lookTime.GetTimeSpan().Seconds;
            }
            return totalTime > Seconds;
        }

        public int GetRemainingTime(SessionHistory history)
        {
            int totalTime = 0;
            foreach (TimePeriod lookTime in history.GetCurrentLevelLookTimePeriods())
            {
                totalTime += lookTime.GetTimeSpan().Seconds;
            }
            return Math.Max(0, Seconds - totalTime);
        }
    }
}

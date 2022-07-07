using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal struct LookObjective : IObjective
    {
        public int Seconds { get; set; }

        public LookObjective(int seconds)
        {
            Seconds = seconds;
        }

        public string Description(SessionHistory history)
        {
            string message = $"Look at the object {Seconds} seconds";

            if (!IsCompleted(history))
            {
                int remainingTime = GetRemainingTime(history);
                message += $" ({remainingTime}s remaining)";
            }
            return message;
        }

        public bool IsCompleted(SessionHistory history)
        {
            return GetRemainingTime(history) == 0;
        }

        private int GetRemainingTime(SessionHistory history)
        {
            double totalTime = 0;
            foreach (TimePeriod lookTime in history.GetCurrentLevelLookTimePeriods())
            {
                totalTime += lookTime.GetTimeSpan().TotalSeconds;
            }

            int floorTime = (int) Math.Floor(totalTime);
            return Math.Max(0, Seconds - floorTime);
        }
    }
}

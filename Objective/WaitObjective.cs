using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal struct WaitObjective : IObjective
    {
        public int Seconds { get; set; }

        public WaitObjective(int seconds)
        {
            Seconds = seconds;
        }

        public string Description(SessionHistory history)
        {
            string details = $"Wait for {Seconds} {(Seconds == 1 ? "second" : "seconds")}";

            if (!IsCompleted(history))
            {
                int remainingTime = (int) Math.Ceiling(Math.Max(0, Seconds - history.GetCurrentLevelTime().TotalSeconds));
                details += $" ({remainingTime}s remaining)";
            }

            return details;
        }

        public bool IsCompleted(SessionHistory history)
        {
            return history.GetCurrentLevelTime().TotalSeconds > Seconds;
        }
    }
}

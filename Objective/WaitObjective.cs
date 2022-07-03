using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal struct WaitObjective : IObjective
    {
        public string Description { get => $"Wait for {Seconds} seconds"; }
        public int Seconds { get; set; }

        public WaitObjective(int seconds)
        {
            Seconds = seconds;
        }

        public bool IsCompleted(SessionHistory history)
        {
            return history.GetCurrentLevelTime().TotalSeconds > Seconds;
        }

        public int GetRemainingTime(SessionHistory history)
        {
            return Math.Max(0, Seconds - history.GetCurrentLevelTime().Seconds);
        }
    }
}

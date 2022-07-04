using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StereoKitApp
{
    internal struct PickUpObjective : IObjective
    {
        public string Description { get => $"Pick up the object"; }

        public PickUpObjective()
        {
        }

        public bool IsCompleted(SessionHistory history)
        {
            return history.GetCurrentLevelHoldTimePeriods().Any();
        }
    }
}

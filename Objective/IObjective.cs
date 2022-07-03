using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    public interface IObjective
    {
        public string Description { get; }
        public bool IsCompleted(SessionHistory history);
    }
}

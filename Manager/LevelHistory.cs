using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    public struct LevelHistory 
    {
        // TODO not sure why this constructor is needed... but sometimes the C# compiler complains!
        public LevelHistory(DateTime begin, int level) : this()
        {
            Begin = begin;
            End = null;
            Level = level;
            MinDistance = float.MaxValue;
        }

        public DateTime Begin { get; set; }
        public DateTime? End { get; set; }
        public int Level { get; set; }
        public float MinDistance { get; set; }

        public TimeSpan GetTimeSpan()
        {
            return (End ?? DateTime.Now) - Begin;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    public struct LevelHistory 
    {
        public LevelHistory(DateTime begin, DateTime? end, int level) : this()
        {
            Begin = begin;
            End = end;
            Level = level;
        }

        public DateTime Begin { get; set; }
        public DateTime? End { get; set; }
        public int Level { get; set; }
        public float MinDistance { get; set; } = float.MaxValue;

        public TimeSpan GetTimeSpan()
        {
            return (End ?? DateTime.Now) - Begin;
        }
    }
}

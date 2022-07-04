using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal struct DistanceObjective : IObjective
    {
        private float _distance;

        public string Description { get => $"Get within {Distance} {(Distance ==  1 ? "meter" : "meters")} of the object"; }
        public float Distance 
        { 
            get => _distance;
            set 
            {
                if (value < 0)
                    throw new ArgumentException("distance must be greater than 0!", nameof(Distance));

                _distance = value;
            }
        }


        public DistanceObjective(float distance)
        {
            if (distance < 0)
                throw new ArgumentException("distance must be greater than 0!", nameof(distance));

            _distance = distance;
        }

        public bool IsCompleted(SessionHistory history)
        {
            return history.GetCurrentLevelHistory().MinDistance < Distance;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal struct DistanceObjective : IObjective
    {
        private float _distance;

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

        public string Description(SessionHistory history)
        {
            if (Distance >= 1)
                return $"Get within {Distance} {(Distance == 1 ? "meter" : "meters")} of the object";

            return $"Put face within {Distance * 100} centimeters of the object";
        }

        public bool IsCompleted(SessionHistory history)
        {
            return history.GetCurrentLevelHistory().MinDistance < Distance;
        }
    }
}

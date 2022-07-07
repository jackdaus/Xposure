using StereoKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StereoKitApp
{
    public class SpidersScene : IScene
    {
        private List<Spider> _spiders = new List<Spider>();
        private List<IObjective> _objectives = new List<IObjective>();

        // TODO turn this into a service?
        private SessionHistory _sessionHistory;

        // Hide the no-parameter constructor for now
        private SpidersScene() { }

        public SpidersScene(SessionHistory history)
        {
            _sessionHistory = history;
        }

        public void Init(int startingLevel)
        {
            _spiders.Clear();
            Spider spider = new Spider();
            spider.Init();

            Vec3 position = new Vec3(0.25f, Util.FloorHeight + 0.05f, 0);

            spider.SetPosition(position);
            spider.PhysicsEnabled = true;
            _spiders.Add(spider);

            // Begin at level 1
            SetCurrentLevel(startingLevel);
        }

        public void Step() 
        {
            _spiders.ForEach(sp => sp.Step());
        }

        public void SetCurrentLevel(int level)
        {
            if (level > GetMaxLevel())
                throw new ArgumentOutOfRangeException();

            switch (level)
            {
                case 1:
                    // Level 1: One stationary M1 spider
                    _spiders.ForEach(sp =>
                    {
                        sp.ModelIntensity = 1;
                        sp.RoamingMode = Roaming.None;
                    });

                    // Objectives
                    _objectives.Clear();
                    _objectives.Add(new WaitObjective(2));
                    //_objectives.Add(new DistanceObjective(0.5f));
                    _objectives.Add(new TouchObjective(1));
                    _objectives.Add(new PickUpObjective());

                    break;
                case 2:
                    // Level 2: One roaming M1 spider
                    _spiders.ForEach(sp =>
                    {
                        sp.ModelIntensity = 9;
                        sp.RoamingMode = Roaming.Walk;
                    });

                    // Objectives
                    _objectives.Clear();
                    _objectives.Add(new WaitObjective(1));
                    _objectives.Add(new LookObjective(10));

                    //_objectives.Add(new TouchObjective(1));

                    break;

                //case 3:
                //    // Level 3: One roaming M5 spider
                //    _spiders.ForEach(sp =>
                //    {
                //        sp.ModelIntensity = 5;
                //        sp.RoamingOn = true;
                //    });

                //    // Objectives
                //    _objectives.Clear();
                //    _objectives.Add(new WaitObjective(5));

                //    break;
                //case 4:
                //    // Level 4: One roaming M9 spider
                //    _spiders.ForEach(sp =>
                //    {
                //        sp.ModelIntensity = 9;
                //        sp.RoamingOn = true;
                //    });

                //    // Objectives
                //    _objectives.Clear();
                //    _objectives.Add(new WaitObjective(7));
                //    _objectives.Add(new TouchObjective(2));

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public int GetMaxLevel()
        {
            return 2; 
        }

        public void Destroy()
        {
            // Nothing to do here :)
        }

        public bool PatientIsTouchingAnyPhobicStimulus()
        {
            foreach (var sp in _spiders)
            {
                if (sp.PatientIsTouching())
                    return true;
            }

            return false;
        }

        public bool PatientIsLookingAtAnyPhobicStimulus()
        {
            foreach (var sp in _spiders)
            {
                if (sp.PatientIsLooking())
                    return true;
            }

            return false;
        }

        public bool PatientIsHoldingAnyPhobicStimulus()
        {
            foreach (var sp in _spiders)
            {
                if (sp.PatientIsHolding())
                    return true;
            }

            return false;
        }

        public float GetMinDistance()
        {
            return _spiders.Min(sp => Vec3.Distance(sp.GetPosition(), Input.Head.position));
        }

        public List<IObjective> GetObjectives()
        {
            // Return copy of list
            return _objectives.ToList();
        }

        public bool AllObjectivesComplete()
        {
            return _objectives.All(o => o.IsCompleted(_sessionHistory));
        }
    }
}

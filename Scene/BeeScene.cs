using StereoKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StereoKitApp
{
    public class BeeScene : IScene
    {
        private List<Bee> _bees = new List<Bee>();
        private List<IObjective> _objectives = new List<IObjective>();

        // TODO turn this into a service?
        private SessionHistory _sessionHistory;

        private BeeScene() { }

        public BeeScene(SessionHistory history)
        {
            _sessionHistory = history;
        }

        public void Init(int startingLevel)
        {
            _bees.Clear();
            Bee bee = new Bee();
            bee.Init();

            // Put bee "in front" if debugging for easier viewing
            var position = DebugTools.DEBUG_TOOLS_ON
                ? new Vec3(0, 0, -0.6f)
                : new Vec3(0.25f, Util.FloorHeight + 0.05f, 0);

            bee.SetPosition(position);
            _bees.Add(bee);

            SetCurrentLevel(startingLevel);
        }

        public void Step() 
        {
            _bees.ForEach(sp => sp.Step());
        }

        public void SetCurrentLevel(int level)
        {
            if (level > GetMaxLevel())
                throw new ArgumentOutOfRangeException();

            switch (level)
            {
                case 1:
                    // Level 1: One stationary M1 bee
                    _bees.ForEach(sp =>
                    {
                        sp.ModelIntensity = 1;
                        sp.RoamingOn = false;
                    });

                    // Objectives
                    _objectives.Clear();
                    _objectives.Add(new WaitObjective(5));

                    break;
                case 2:
                    // Level 2: One roaming M1 bee
                    _bees.ForEach(sp =>
                    {
                        sp.ModelIntensity = 1;
                        sp.RoamingOn = true;
                    });

                    // Objectives
                    _objectives.Clear();
                    _objectives.Add(new WaitObjective(8));
                    _objectives.Add(new TouchObjective(1));

                    break;

                case 3:
                    // Level 3: One roaming M5 bee
                    _bees.ForEach(sp =>
                    {
                        sp.ModelIntensity = 2;
                        sp.RoamingOn = true;
                    });

                    // Objectives
                    _objectives.Clear();
                    _objectives.Add(new WaitObjective(5));

                    break;
                case 4:
                    // Level 4: One roaming M4 bee
                    _bees.ForEach(sp =>
                    {
                        sp.ModelIntensity = 4;
                        sp.RoamingOn = true;
                    });

                    // Objectives
                    _objectives.Clear();
                    _objectives.Add(new WaitObjective(7));
                    _objectives.Add(new TouchObjective(2));

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public int GetMaxLevel()
        {
            return 4; 
        }

        public bool HandIsTouchingAnyPhobicStimulus()
        {
            foreach (var b in _bees)
            {
                if (b.HandIsTouching())
                    return true;
            }

            return false;
        }

        public bool PatientIsLookingAtAnyPhobicStimulus()
        {
            foreach (var b in _bees)
            {
                if (b.PatientIsLooking())
                    return true;
            }

            return false;
        }

        public float GetMinDistance()
        {
            return _bees.Min(b => Vec3.Distance(b.GetPosition(), Input.Head.position));
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

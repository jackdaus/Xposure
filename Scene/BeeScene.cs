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

            // Disable the physics so the bee flies!
            bee.PhysicsEnabled = false;

            Vec3 position =  new Vec3(0.25f, Util.FloorHeight + 1.5f, -0.6f);

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
                    // Start basic. Just a solitary stationary sphere.
                    _bees.ForEach(b =>
                    {
                        b.ModelIntensity = 1;
                        b.RoamingMode = Roaming.None;
                        b.SoundEnabled = false;
                    });

                    // Objectives
                    _objectives.Clear();
                    _objectives.Add(new LookObjective(5));

                    break;
                case 2:
                    // Increase model realism.
                    _bees.ForEach(b =>
                    {
                        b.ModelIntensity = 2;
                        b.RoamingMode = Roaming.None;
                        b.SoundEnabled = false;
                    });

                    // Objectives
                    _objectives.Clear();
                    _objectives.Add(new DistanceObjective(1f));
                    _objectives.Add(new PickUpObjective());

                    break;

                case 3:
                    // Add motion.
                    _bees.ForEach(b =>
                    {
                        b.ModelIntensity = 2;
                        b.RoamingMode = Roaming.Fly;
                        b.SoundEnabled = false;
                    });

                    // Objectives
                    _objectives.Clear();
                    _objectives.Add(new LookObjective(5));

                    break;
                case 4:
                    // Increase model realism and add sound
                    _bees.ForEach(b =>
                    {
                        b.ModelIntensity = 3;
                        b.RoamingMode = Roaming.Fly;
                        b.SoundEnabled = true;
                    });

                    // Objectives
                    _objectives.Clear();
                    _objectives.Add(new WaitObjective(5));

                    break;
                case 5:
                    // Increase model realism
                    _bees.ForEach(b =>
                    {
                        b.ModelIntensity = 4;
                        b.RoamingMode = Roaming.Fly;
                        b.SoundEnabled = true;
                        b.AnimationEnabled = true;
                    });

                    // Objectives
                    _objectives.Clear();
                    _objectives.Add(new LookObjective(5));

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public int GetMaxLevel()
        {
            return 5;
        }

        public void Destroy()
        {
            // Turn off sounds in case any are on!
            _bees.ForEach(b =>
            {
                b.SoundEnabled = false;
                b.Destroy();
            });
        }

        public bool PatientIsTouchingAnyPhobicStimulus()
        {
            foreach (var b in _bees)
            {
                if (b.PatientIsTouching())
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

        public bool PatientIsHoldingAnyPhobicStimulus()
        {
            foreach (var b in _bees)
            {
                if (b.PatientIsHolding())
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

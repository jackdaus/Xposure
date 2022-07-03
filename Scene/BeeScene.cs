using StereoKit;
using StereoKitApp.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    public class BeeScene : IScene
    {
        private int _currentLevel = 0;
        private List<Bee> _bees = new List<Bee>();
        private Objective[] _levelObjectives = new Objective[9];
        public BeeScene()
        {
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
            bee.Level = startingLevel;
            _bees.Add(bee);
        }

        public void Step() 
        {
            _bees.ForEach(sp => sp.Step());
            _levelObjectives[this._currentLevel].Step();
        }

        public void SetCurrentLevel(int level)
        {
            _currentLevel = level;
            _bees.ForEach(sp => sp.Level = level);
        }

        public int GetMaxLevel()
        {
            // TODO design actual levels
            return 4; 
        }

        public void setObjective( int level, int type, int goal )
        {
            _levelObjectives[level-1] = new Objective(type, goal);
        }

        public bool IsObjectiveCompleted( int level )
        {
            return _levelObjectives[level - 1].IsGoalReached;
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
    }
}

using StereoKit;
using StereoKitApp.Scene;
using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    public class SpidersScene : IScene
    {
        private int _currentLevel = 0;
        private List<Spider> spiders = new List<Spider>();
        private Objective[] levelObjectives = new Objective[9];
        public SpidersScene()
        {
        }

        public void Init(int startingLevel)
        {
            spiders.Clear();
            Spider spider = new Spider();
            spider.Init();

            // Put spider "in front" if debugging for easier viewing
            var position = DebugTools.DEBUG_TOOLS_ON
                ? new Vec3(0, 0, -0.6f)
                : new Vec3(0.25f, Util.FloorHeight + 0.05f, 0);

            spider.SetPosition(position);
            spider.Level = startingLevel;
            spiders.Add(spider);
        }

        public void Step() 
        {
            spiders.ForEach(sp => sp.Step());
            levelObjectives[this._currentLevel].Step();
        }

        public void SetCurrentLevel(int level)
        {
            this._currentLevel = level;
            spiders.ForEach(sp => sp.Level = level);
        }

        public int GetMaxLevel()
        {
            // TODO design actual levels
            return 9; 
        }

        public void setObjective( int level, int type, int goal )
        {
            levelObjectives[level-1] = new Objective(type, goal);
        }

        public bool IsObjectiveCompleted( int level )
        {
            return this.levelObjectives[level - 1].IsGoalReached;
        }

        public bool HandIsTouchingAnyPhobicStimulus()
        {
            foreach (var sp in spiders)
            {
                if (sp.HandIsTouching())
                    return true;
            }

            return false;
        }

        public bool PatientIsLookingAtAnyPhobicStimulus()
        {
            foreach (var sp in spiders)
            {
                if (sp.PatientIsLooking())
                    return true;
            }

            return false;
        }
    }
}

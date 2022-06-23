using StereoKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal class SpidersScene
    {
        private List<Spider> spiders = new List<Spider>();
        private int currentLevel = 0;

        public SpidersScene()
        {
        }

        public void setCurrentLevel(int level)
        {
            this.currentLevel = level;
        }
        public void init(int startingLevel)
        {
            spiders.Clear();
            Spider spider = new Spider();
            spider.SetPosition(0.5f, World.HasBounds ? World.BoundsPose.position.y : -1.5f + 0.05f, -2);
            spider.Level = startingLevel;
            spiders.Add(spider);
        }
        public void Step() 
        {
            foreach (var spider in spiders)
            {
                spider.Step();
            }
        }
    }
}

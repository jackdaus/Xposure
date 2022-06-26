using StereoKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal class SpidersScene
    {
        private List<Spider> spiders = new List<Spider>();

        public SpidersScene()
        {
        }

        public void Init(int startingLevel)
        {
            spiders.Clear();
            Spider spider = new Spider();
            spider.SetPosition(0f, Util.FloorHeight + 0.05f, 0);
            spider.Level = startingLevel;
            spiders.Add(spider);
        }

        public void Step() 
        {
            spiders.ForEach(sp => sp.Step());
        }

        public void SetCurrentLevel(int level)
        {
            spiders.ForEach(sp => sp.Level = level);
        }
    }
}

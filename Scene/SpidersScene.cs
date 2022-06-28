using StereoKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    public class SpidersScene : IScene
    {
        private List<Spider> spiders = new List<Spider>();

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
                : new Vec3(0, Util.FloorHeight + 0.05f, -1);

            spider.SetPosition(position);
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

        public int GetMaxLevel()
        {
            // TODO design actual levels
            return 9; 
        }
    }
}

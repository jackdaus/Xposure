using System;
using System.Collections.Generic;
using System.Text;
using StereoKit;

namespace StereoKitApp
{
    internal class LevelManager
    {
        public Pose pose { get; set; }
        public Vec2 dimensions { get; set; }

        public LevelManager(Pose pose, Vec2 dimensions)
        {
            this.pose = pose;
            this.dimensions = dimensions;
        }
        public void Step()
        {
            Pose pose = this.pose;

            UI.WindowBegin("Level selector", ref pose, new Vec2(20, 0) * U.cm, UIWin.Normal, UIMove.Exact);
            UI.Label("Spiders");
            UI.Button("Level 1");
            UI.SameLine();
            UI.Button("Level 2");
            UI.SameLine();
            UI.Button("Level 3");

            UI.WindowEnd();

            if (!pose.position.Equals(this.pose.position) || !pose.orientation.Equals(this.pose.orientation))
                this.pose = pose;
        }
    }
}

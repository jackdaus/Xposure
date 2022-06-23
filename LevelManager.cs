using System;
using System.Collections.Generic;
using System.Text;
using StereoKit;

namespace StereoKitApp
{
    internal class LevelManager
    {
        private bool[] selectedScene = {false};
        private bool sceneStarted = false;
        private SpidersScene spidersScene = new SpidersScene();
        private Pose pose { get; set; }
        private int currentLevel = 0;
        private const int SPIDERS_MAX_LEVEL = Spider.MAX_LEVEL;

        public LevelManager(Pose pose, Vec2 dimensions)
        {
            this.pose = pose;
        }
        public void Step()
        {
            Pose pose = this.pose;

            UI.WindowBegin("Level manager", ref pose, new Vec2(20, 10) * U.cm, UIWin.Normal, UIMove.Exact);
            if (!sceneStarted)
            {
                UI.Label("Scenario");
                UI.Toggle("Spiders", ref selectedScene[0]);
                if (selectedScene[0])
                {
                    UI.Label($"Starting level: {currentLevel} out of {SPIDERS_MAX_LEVEL}");
                    if (UI.Button("Down") && currentLevel > 0)
                    {
                        currentLevel--;
                    }
                    UI.SameLine();
                    if (UI.Button("Up") && currentLevel < SPIDERS_MAX_LEVEL)
                    {
                        currentLevel++;
                    }
                    UI.SameLine();
                    if (UI.Button("Start"))
                    {
                        spidersScene.init(currentLevel);
                        sceneStarted = true;
                    }
                }
            } else
            {
                UI.Toggle("Stop", ref sceneStarted);
            }

            UI.WindowEnd();
            if (sceneStarted)
            {
                stepScene();
            }

            if (!pose.position.Equals(this.pose.position) || !pose.orientation.Equals(this.pose.orientation))
                this.pose = pose;
        }

        private void stepScene()
        {
            switch (Array.IndexOf(selectedScene, true))
            {
                case 0:
                    spidersScene.Step();
                    break;
                default:
                    Log.Info("No scene selected!");
                    break;
            }
        }
    }
}

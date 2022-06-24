using System;
using System.Collections.Generic;
using System.Text;
using StereoKit;

namespace StereoKitApp
{
    internal class LevelManager
    {
        private PhobiaType? selectedPhobiaType;

        private SpidersScene spidersScene = new SpidersScene();
        private int currentLevel = 0;
        private const int SPIDERS_MAX_LEVEL = Spider.MAX_LEVEL;

        Pose windowPose = new Pose(-0.4f, 0, -0.4f, Quat.LookDir(1, 0, 1));

        public LevelManager()
        { 
        }

        public void Step()
        {
            UI.WindowBegin("Level Manager", ref windowPose, new Vec2(20, 10) * U.cm, UIWin.Normal, UIMove.Exact);

            UI.Label("Start a scenario");
            if (UI.Button("Spider"))    initScene(PhobiaType.Spider);
            UI.SameLine();
            if (UI.Button("Bee"))       initScene(PhobiaType.Bee);
            UI.SameLine();
            if (UI.Button("Bird"))      initScene(PhobiaType.Bird);

            if (selectedPhobiaType.HasValue)
            {
                UI.HSeparator();
                UI.Label($"Selected: {selectedPhobiaType}");

                UI.Label($"Level {currentLevel} out of {SPIDERS_MAX_LEVEL}");
                if (UI.Button("Down") && currentLevel > 0)
                {
                    currentLevel--;
                    spidersScene.SetCurrentLevel(currentLevel);
                }
                UI.SameLine();
                if (UI.Button("Up") && currentLevel < SPIDERS_MAX_LEVEL)
                {
                    currentLevel++;
                    spidersScene.SetCurrentLevel(currentLevel);
                }
                UI.SameLine();

                if (UI.Button("Stop")) stopScene();
            }
            UI.WindowEnd();

            if (selectedPhobiaType.HasValue) spidersScene.Step();
        }

        private void initScene(PhobiaType type)
        {
            selectedPhobiaType = type;
            spidersScene.Init(currentLevel);
        }

        private void stopScene()
        {
            currentLevel = 0;
            selectedPhobiaType = null;
            spidersScene.SetCurrentLevel(currentLevel);
        }

        private enum PhobiaType
        {
            Spider,
            Bee,
            Bird,
            Snake,
            Dogs,
            Insect,
            Reptile,
        }
    }
}

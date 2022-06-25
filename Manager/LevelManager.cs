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
        
        SessionHistory history = new SessionHistory();

        Color Green = new Color(0.5f, 1f, 0.5f);

        public LevelManager()
        { 
        }

        public void Step()
        {
            UI.WindowBegin("XposuRe", ref windowPose, new Vec2(20, 10) * U.cm, UIWin.Normal, UIMove.FaceUser);

            // TODO better disabled buttons. They still don't quite look disabled
            UI.Label("Start a scenario");
            if (UIExtensions.Button("Spider",
                disabled: selectedPhobiaType.HasValue,
                color: selectedPhobiaType == PhobiaType.Spider ? Green : null))
                initScene(PhobiaType.Spider);
            UI.SameLine();
            if (UIExtensions.Button("Bee",
                disabled: selectedPhobiaType.HasValue,
                color: selectedPhobiaType == PhobiaType.Bee ? Green : null))
                initScene(PhobiaType.Bee);
            UI.SameLine();
            if (UIExtensions.Button("Bird",
                disabled: selectedPhobiaType.HasValue,
                color: selectedPhobiaType == PhobiaType.Bird ? Green : null))
                initScene(PhobiaType.Bird);

            if (selectedPhobiaType.HasValue)
            {
                UI.HSeparator();
                //UI.Label($"Selected: {selectedPhobiaType}");

                UI.Label($"Level {currentLevel} out of {SPIDERS_MAX_LEVEL}");

                if (UI.Button("Down") && currentLevel > 0) changeLevel(currentLevel - 1);
                UI.SameLine();
                if (UI.Button("Up") && currentLevel < SPIDERS_MAX_LEVEL) changeLevel(currentLevel + 1);
                UI.SameLine();

                if (UI.Button("Stop")) stopScene();

                TimeSpan currentLevelTime = history.GetCurrentLevelTime();
                UI.Label($"Level time: {currentLevelTime.Minutes}m {currentLevelTime.Seconds}s");

                var totalTime = history.GetTotalTime();
                UI.Label($"Total time: {totalTime.Minutes}m {totalTime.Seconds}s");
            }
            UI.WindowEnd();

            if (selectedPhobiaType.HasValue) spidersScene.Step();
        }

        private void initScene(PhobiaType type)
        {
            if (selectedPhobiaType != null)
                throw new InvalidOperationException("Scene already in progress! Cannot begin a new scene.");
            
            history.BeginSession(currentLevel);

            selectedPhobiaType = type;
            spidersScene.Init(currentLevel);
        }

        private void stopScene()
        {
            history.EndSession();

            currentLevel = 0;
            spidersScene.SetCurrentLevel(0);
            selectedPhobiaType = null;
        }

        private void changeLevel(int level)
        {
            currentLevel = level;
            spidersScene.SetCurrentLevel(currentLevel);

            history.EndSession();
            history.BeginSession(currentLevel);
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

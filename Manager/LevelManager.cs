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
        Report _report = new Report();

        public LevelManager()
        { 
        }

        public void Step()
        {
            if (selectedPhobiaType.HasValue) 
                spidersScene.Step();

            _report.Step(history);
            
            UI.WindowBegin("XposuRe", ref windowPose);

                if (!selectedPhobiaType.HasValue)
                {
                    UI.Label("Start a scenario");
                    if (UI.Button("Spider"))    
                        initScene(PhobiaType.Spider);
                    UI.SameLine();
                    if (UI.Button("Bee"))       
                        initScene(PhobiaType.Bee);
                    UI.SameLine();
                    if (UI.Button("Bird"))      
                        initScene(PhobiaType.Bird);
                }

                if (selectedPhobiaType.HasValue)
                {
                    UI.Label($"Level {currentLevel} out of {SPIDERS_MAX_LEVEL}");
                    UI.SameLine();
                    if (UI.ButtonRound("Down", Asset.Instance.IconDown) && currentLevel > 0) 
                        changeLevel(currentLevel - 1);
                    UI.SameLine();
                    if (UI.ButtonRound("Up", Asset.Instance.IconUp) && currentLevel < SPIDERS_MAX_LEVEL) 
                        changeLevel(currentLevel + 1);

                    if (UI.Button("Done")) 
                        stopScene();

                    TimeSpan currentLevelTime = history.GetCurrentLevelTime();
                    UI.SameLine();
                    UI.Label($"{currentLevelTime.Minutes}m {currentLevelTime.Seconds}s");
                }


                if (history.Any() && !selectedPhobiaType.HasValue && !_report.IsVisible())
                {
                    UI.HSeparator();
                    if (UI.Button("View Report"))
                        _report.MakeVisible(windowPose);
                }

            UI.WindowEnd();
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

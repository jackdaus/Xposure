using System;
using System.Collections.Generic;
using System.Text;
using StereoKit;

namespace StereoKitApp
{
    internal class LevelManager
    {
        private PhobiaType? _selectedPhobiaType;

        private SpidersScene _spidersScene = new SpidersScene();
        private int _currentLevel = 0;
        private const int SPIDERS_MAX_LEVEL = Spider.MAX_LEVEL;

        Pose _windowPose = new Pose(-0.4f, 0, -0.4f, Quat.LookDir(1, 0, 1));

        SessionHistory _history = new SessionHistory();
        Report _report = new Report();

        public LevelManager()
        { 
        }

        public void Step()
        {
            if (_selectedPhobiaType.HasValue) 
                _spidersScene.Step();

            _report.Step(_history);
            
            UI.WindowBegin("XposuRe", ref _windowPose);

                if (!_selectedPhobiaType.HasValue)
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

                if (_selectedPhobiaType.HasValue)
                {
                    UI.Label($"Level {_currentLevel} out of {SPIDERS_MAX_LEVEL}");
                    UI.SameLine();
                    if (UI.ButtonRound("Down", Asset.Instance.IconDown) && _currentLevel > 0) 
                        changeLevel(_currentLevel - 1);
                    UI.SameLine();
                    if (UI.ButtonRound("Up", Asset.Instance.IconUp) && _currentLevel < SPIDERS_MAX_LEVEL) 
                        changeLevel(_currentLevel + 1);

                    if (UI.Button("Done")) 
                        stopScene();

                    TimeSpan currentLevelTime = _history.GetCurrentLevelTime();
                    UI.SameLine();
                    UI.Label($"{currentLevelTime.Minutes}m {currentLevelTime.Seconds}s");
                }


                if (_history.Any() && !_selectedPhobiaType.HasValue && !_report.IsVisible())
                {
                    UI.HSeparator();
                    if (UI.Button("View Report"))
                        _report.MakeVisible(_windowPose);
                }

            UI.WindowEnd();
        }

        private void initScene(PhobiaType type)
        {
            if (_selectedPhobiaType != null)
                throw new InvalidOperationException("Scene already in progress! Cannot begin a new scene.");
            
            _history.BeginSession(_currentLevel);

            _selectedPhobiaType = type;
            _spidersScene.Init(_currentLevel);
        }

        private void stopScene()
        {
            _history.EndSession();

            _currentLevel = 0;
            _spidersScene.SetCurrentLevel(0);
            _selectedPhobiaType = null;
        }

        private void changeLevel(int level)
        {
            _currentLevel = level;
            _spidersScene.SetCurrentLevel(_currentLevel);

            _history.EndSession();
            _history.BeginSession(_currentLevel);
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

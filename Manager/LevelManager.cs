using System;
using System.Collections.Generic;
using System.Text;
using StereoKit;

namespace StereoKitApp
{
    internal class LevelManager
    {
        private PhobiaType? _selectedPhobiaType;
        private IScene _scene;
        private int _currentSceneLevel = 0;
        private Pose _windowPose = new Pose(-0.4f, 0, -0.4f, Quat.LookDir(1, 0, 1));
        private SessionHistory _history = new SessionHistory();
        private Report _report = new Report();

        public LevelManager()
        { 
        }

        public void Step()
        {
            if (_selectedPhobiaType.HasValue) 
                _scene.Step();

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
                    UI.Label($"Level {_currentSceneLevel} out of {_scene.GetMaxLevel()}");
                    UI.SameLine();
                    if (UI.ButtonRound("Down", Asset.Instance.IconDown) && _currentSceneLevel > 0) 
                        changeLevel(_currentSceneLevel - 1);
                    UI.SameLine();
                    if (UI.ButtonRound("Up", Asset.Instance.IconUp) && _currentSceneLevel < _scene.GetMaxLevel()) 
                        changeLevel(_currentSceneLevel + 1);

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

            _selectedPhobiaType = type;

            switch (type)
            {
                case PhobiaType.Spider:
                    _scene = new SpidersScene();
                    break;
                case PhobiaType.Bee:
                    // TODO
                    _scene = new SpidersScene();
                    break;
                case PhobiaType.Dog:
                    // TODO
                    _scene = new SpidersScene();
                    break;
                defaut:
                    throw new NotImplementedException();
                    break;
            }

            _scene.Init(_currentSceneLevel);
            _history.BeginSession(_currentSceneLevel);
        }

        private void stopScene()
        {
            _history.EndSession();

            _currentSceneLevel = 0;
            _scene.SetCurrentLevel(0);
            _selectedPhobiaType = null;
        }

        private void changeLevel(int level)
        {
            _currentSceneLevel = level;
            _scene.SetCurrentLevel(_currentSceneLevel);

            _history.EndSession();
            _history.BeginSession(_currentSceneLevel);
        }

        private enum PhobiaType
        {
            Spider,
            Bee,
            Bird,
            Snake,
            Dog,
            Insect,
            Reptile,
        }
    }
}

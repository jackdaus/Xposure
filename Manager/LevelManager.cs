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
        private int _currentSceneLevel = 1;
        private Pose _windowPose = new Pose(-0.3f, 0, -0.3f, Quat.LookDir(1, 0, 1));
        private SessionHistory _history = new SessionHistory();
        private Report _report = new Report();
        private bool _wasJustTouching = false;
        private bool _wasJustLooking = false;

        public LevelManager()
        { 
        }

        public void Step()
        {
            if (_selectedPhobiaType.HasValue)
            {
                _scene.Step();
                recordTouches();
                recordLooks();
            }

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
                    if (UI.Button("Claustrophobia"))
                        initScene(PhobiaType.Claustrophobia);
                }
                else
                {
                    UI.Label($"Level {_currentSceneLevel} out of {_scene.GetMaxLevel()}");
                    UI.SameLine();
                    if (_scene.IsObjectiveCompleted(_currentSceneLevel) && _currentSceneLevel < _scene.GetMaxLevel())
                    {
                        if(UI.ButtonRound("Up", Asset.Instance.IconUp))
                            changeLevel(_currentSceneLevel + 1);
                    }
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
                    _scene.setObjective(1, 1, 5);
                    _scene.setObjective(2, 2, 5);
                    _scene.setObjective(3, 1, 15);
                    _scene.setObjective(4, 1, 20);
                    _scene.setObjective(5, 2, 20);
                    _scene.setObjective(6, 1, 30);
                    _scene.setObjective(7, 1, 10);
                    _scene.setObjective(8, 2, 20);
                    _scene.setObjective(9, 1, 30);
                    break;
                case PhobiaType.Bee:
                    // TODO
                    _scene = new SpidersScene();
                    break;
                case PhobiaType.Claustrophobia:
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

            _currentSceneLevel = 1;
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

        private void recordTouches()
        {
            // TODO add a debounce
            if (_scene.HandIsTouchingAnyPhobicStimulus())
            {
                if (!_wasJustTouching)
                {
                    // Just touched!
                    //Log.Info($"Just Touched! {DateTime.Now.Ticks}");
                    _history.BeginTouchPeriod();

                    _wasJustTouching = true;
                }
            }
            else if (_wasJustTouching)
            {
                // Just untouched!
                //Log.Info($"Just Untouched! {DateTime.Now.Ticks}");
                _history.EndTouchPeriod();

                _wasJustTouching = false;
            }
        }

        private void recordLooks()
        {
            // TODO add a debounce
            if (_scene.PatientIsLookingAtAnyPhobicStimulus())
            {
                if (!_wasJustLooking)
                {
                    // Just touched!
                    //Log.Info($"Just Touched! {DateTime.Now.Ticks}");
                    _history.BeginLookPeriod();

                    _wasJustLooking = true;
                }
            }
            else if (_wasJustLooking)
            {
                // Just untouched!
                //Log.Info($"Just Untouched! {DateTime.Now.Ticks}");
                _history.EndLookPeriod();

                _wasJustLooking = false;
            }
        }

        private enum PhobiaType
        {
            Spider,
            Bee,
            Claustrophobia,
        }
    }
}

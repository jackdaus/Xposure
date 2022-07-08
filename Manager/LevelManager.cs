using System;
using System.Collections.Generic;
using System.Text;
using StereoKit;
using StereoKit.Framework;

namespace StereoKitApp
{
    internal class LevelManager : IStepper
    {
        private Phobia? _selectedPhobiaType;
        private IScene _scene;
        private int _currentSceneLevel = 1;
        private Pose _windowPose = new Pose(-0.3f, 0, -0.3f, Quat.LookDir(1, 0, 1));
        private SessionHistory _history = new SessionHistory();
        private Report _report = new Report();
        private bool _wasJustTouching = false;
        private bool _wasJustLooking = false;
        private bool _wasJustHolding = false;

        public LevelManager()
        { 
        }

        public bool Enabled { get; }

        public bool Initialize()
        {
            return true;
        }

        public void Shutdown()
        {
        }

        public void Step()
        {
            if (_selectedPhobiaType.HasValue)
            {
                _scene.Step();
                recordTouches();
                recordLooks();
                recordMinDistance();
                recordHolds();
            }

            _report.Step(_history);

            UI.WindowBegin("Xposure Therapy", ref _windowPose);

            // Passthrough toggle
            if (App.Passthrough.Available)
            {
                bool toggle = App.Passthrough.EnabledPassthrough;
                if (UI.ButtonRound("Passthrough", Asset.Instance.IconEye))
                    App.Passthrough.EnabledPassthrough = !App.Passthrough.EnabledPassthrough;
                UI.HSeparator();
            }
                
            // Phobia selection
            if (!_selectedPhobiaType.HasValue)
            {
                UI.Label("Start a scenario");
                if (UI.Button("Spider"))    
                    initScene(Phobia.Spider);
                UI.SameLine();
                if (UI.Button("Bee"))       
                    initScene(Phobia.Bee);

                // Quit button
                UI.HSeparator();
                UI.PushTint(Util.Colors.Red);
                    if (UI.ButtonRound("shut_down", Asset.Instance.IconPower))
                        SK.Quit();
                UI.PopTint();

                UI.SameLine();
                if (UI.Button("Credits"))
                    App.CreditsStepper.Enabled = !App.CreditsStepper.Enabled;

                // View report button
                if (_history.Any() && !_report.IsVisible())
                {
                    UI.SameLine();
                    if (UI.Button("View Report"))
                        _report.MakeVisible(_windowPose);
                }
            }
            // Level controls
            else
            {
                UI.Label($"Level {_currentSceneLevel} out of {_scene.GetMaxLevel()}");

                // Always show the Up button during debug for easier testing (but make it gray to distinguish it)
                if (App.DebugToolsStepper.Enabled && _currentSceneLevel < _scene.GetMaxLevel())
                {
                    UI.SameLine();
                    if (UI.ButtonRound("Up", Asset.Instance.IconUp))
                        changeLevel(_currentSceneLevel + 1);
                }

                // Real up button
                if (_scene.AllObjectivesComplete() && _currentSceneLevel < _scene.GetMaxLevel())
                {
                    UI.SameLine();
                    UI.PushTint(Util.Colors.Green);
                    if (UI.ButtonRound("Up", Asset.Instance.IconUp))
                        changeLevel(_currentSceneLevel + 1);
                    UI.PopTint();
                }

                UI.Label("Objectives:");
                _scene.GetObjectives().ForEach(o =>
                {
                    // Quick fix to fallback when MDL2 symbols are not available
                    if (App.IsAndroid)
                    {
                        UI.Label((o.IsCompleted(_history) ? "[ x ]" : "[   ]")
                            + $"\t{ o.Description(_history)}");
                    }
                    else
                    {
                        UI.Label((o.IsCompleted(_history) ? Util.SpecialChars.CheckboxCompositeReversed : Util.SpecialChars.Checkbox)
                            + $"\t{ o.Description(_history)}");
                    }
                });

                // View Report
                if (_currentSceneLevel == _scene.GetMaxLevel() && _scene.AllObjectivesComplete())
                {
                    UI.Label("");
                    UI.Label("Completed!");
                    if (!_report.IsVisible())
                    {
                        UI.SameLine();
                        if (UI.Button("View Report"))
                            _report.MakeVisible(_windowPose);
                    }
                }

                UI.HSeparator();
                if (UI.Button("Exit")) 
                    stopScene();
            }

            UI.WindowEnd();
        }

        private void initScene(Phobia type)
        {
            if (_selectedPhobiaType != null)
                throw new InvalidOperationException("Scene already in progress! Cannot begin a new scene.");

            _selectedPhobiaType = type;

            // Reset existing history to reset report
            _history.ResetHistory();

            switch (type)
            {
                case Phobia.Spider:
                    _scene = new SpidersScene(_history);
                    break;
                case Phobia.Bee:
                    _scene = new BeeScene(_history);
                    break;
            }

            _scene.Init(_currentSceneLevel);
            _history.BeginLevel(_currentSceneLevel);
        }

        private void stopScene()
        {
            _history.EndLevel();

            _wasJustHolding = false;
            _wasJustLooking = false;
            _wasJustTouching = false;
            
            _currentSceneLevel = 1;
            _selectedPhobiaType = null;
            _scene.Destroy();
        }

        private void changeLevel(int level)
        {
            _currentSceneLevel = level;
            _scene.SetCurrentLevel(_currentSceneLevel);

            _history.EndLevel();
            _history.BeginLevel(_currentSceneLevel);
        }

        private void recordTouches()
        {
            // TODO add a debounce
            if (_scene.PatientIsTouchingAnyPhobicStimulus())
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
                    // Just looked!
                    _history.BeginLookPeriod();

                    _wasJustLooking = true;
                }
            }
            else if (_wasJustLooking)
            {
                // Just looked!
                _history.EndLookPeriod();

                _wasJustLooking = false;
            }
        }

        private void recordHolds()
        {
            // TODO add a debounce
            if (_scene.PatientIsHoldingAnyPhobicStimulus())
            {
                if (!_wasJustHolding)
                {
                    // Just Held!
                    _history.BeginHoldPeriod();

                    _wasJustHolding = true;
                }
            }
            else if (_wasJustHolding)
            {
                // Just Unheld!
                _history.EndHoldPeriod();

                _wasJustHolding = false;
            }
        }

        private void recordMinDistance()
        {
            if (_history.HasActiveLevel())
            {
                var currentMin = _scene.GetMinDistance();

                LevelHistory currentHist = _history.GetCurrentLevelHistory();
                if (currentMin < currentHist.MinDistance)
                {
                    _history.UpdateLevelMinDistance(currentMin);
                }
            }
        }
    }
}

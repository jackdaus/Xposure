using System;
using System.Collections.Generic;
using System.Text;
using StereoKit;

namespace StereoKitApp.Scene
{
    internal class Objective
    {
        private int _type = 0;
        private int _goal = 0;
        private bool _isGoalReached = false;
        public bool IsGoalReached { get => _isGoalReached; }
        private String _goalMessage = "No objectives";
        public String GoalMessage { get => _goalMessage; }

        private DateTime _lastGoalUpdate = DateTime.Now;

        // debug
        private Pose _debugWindowPose = new Pose(-0.3f, 0.6f, 0f, Quat.LookDir(0, 0, 1));
        private Guid _id = Guid.NewGuid();
        public Objective(int type, int goal)
        {
            if (type > 0)
                this._type = type;
            if (goal > 0)
                this._goal = goal;
            Init();
        }
        private void Init()
        {
            
        }

        /// <summary>
        /// Step the objective.
        /// </summary>
        public void Step()
        {
            if (this._goal > 0)
            {
                switch (this._type)
                {
                    // Wait objective
                    case 1:
                        _goalMessage = ($"Wait {this._goal} seconds");
                        // Update goal every second
                        if ((DateTime.Now - _lastGoalUpdate).TotalSeconds >= 1)
                        {
                            this._goal--;
                            _lastGoalUpdate = DateTime.Now;
                        }
                        break;
                    // Look at PhobicStimulus objective
                    case 2:
                        _goalMessage = ($"Look at PHOBIA for {this._goal} seconds");
                        // Update goal every second
                        if ((DateTime.Now - _lastGoalUpdate).TotalSeconds >= 1)
                        {
                            this._goal--;
                            _lastGoalUpdate = DateTime.Now;
                        }
                        break;
                    // Touch PhobicStimulus objective
                    case 3:
                        _goalMessage = ($"Touch PHOBIA {this._goal} times");
                        break;
                    default:
                        _goalMessage = "Objective not found";
                        break;
                }
            }
            else
            {
                this._isGoalReached = true;
                this._goalMessage = "Congratulations, you can now go to the next level!";
            }
            
            // Window for debug controls
            UI.WindowBegin($"OBJ_DEBUG_{_id}", ref _debugWindowPose);
            UI.Label($"Type: {this._type}");
            UI.Label($"Message: {this.GoalMessage}");
            if (this.IsGoalReached && UI.ButtonRound($"Next_{_id}", Asset.Instance.IconClose))
                UI.Label("Done.");
            UI.WindowEnd();
        }

    }
}

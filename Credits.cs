using StereoKit.Framework;
using StereoKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace StereoKitApp
{
    internal class Credits : IStepper
    {
        private Pose _pose = new Pose();
        private bool _enabled;
        private TextStyle _style;

        public bool Enabled 
        { 
            get => _enabled; 
            set
            {
                if (value)
                {
                    Vec3 windowPosition = Input.Head.position + Input.Head.Forward * 0.3f;
                    Quat lookAt = Quat.LookAt(windowPosition, Input.Head.position);
                    _pose = new Pose(windowPosition, lookAt);
                }

                _enabled = value;
            }
        }

        public bool Initialize()
        {
            _style = Text.MakeStyle(Default.Font, 0.012f, Color.White);
            return true;
        }

        public void Shutdown()
        {
        }

        public void Step()
        {
            if (Enabled)
            {
                UI.WindowBegin("Credits", ref _pose, moveType: UIMove.FaceUser);

                if (UI.Button("Close"))
                    Enabled = false;

                UI.HSeparator();

                UI.PushTextStyle(_style);
                UI.Label("Approximate Spider Models");
                UI.PopTextStyle();
                UI.Label("Created by Brianna Baker");

                UI.HSeparator();

                UI.PushTextStyle(_style);
                UI.Label("Bee Buzz Sound");
                UI.PopTextStyle();
                UI.Label("By https://quicksounds.com/sound/2760/bumblebee-1");

                UI.HSeparator();

                UI.PushTextStyle(_style);
                UI.Label("Realistic Bee Model");
                UI.PopTextStyle();
                UI.Label("Exported from Microsoft 3D Viewer");

                UI.HSeparator();

                UI.PushTextStyle(_style);
                UI.Label("Realistic Spider Model");
                UI.PopTextStyle();
                UI.Label("This work is based on \"Animated Low - Poly Spider Game - Ready\" " +
                    Environment.NewLine +
                    "(https://sketchfab.com/3d-models/animated-low-poly-spider-game-ready" +
                    Environment.NewLine +
                    "-2d79c585b5404a23b7bcc0be06068283) by 3DHaupt (https://sketchfab.com/dennish2010)" +
                    Environment.NewLine +
                    "licensed under CC-BY-NC-ND-4.0 (http://creativecommons.org/licenses/by-nc-nd/4.0/)");

                UI.WindowEnd();
            }
        }
    }
}

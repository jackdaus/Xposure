using System;
using System.Collections.Generic;
using System.Text;
using StereoKit;

namespace StereoKitApp
{
    internal class Report
    {
        Pose _pose;
        bool _windowVisible;

        Color Red = new Color(1f, 0, 0); // TODO move to shared area

        public void MakeVisible(Pose basePose)
        {
            var reportPose = basePose;
            // TODO put window in a better location
            reportPose.position.x += 0.1f;

            _pose = reportPose; 
            _windowVisible = true;
        }

        public bool IsVisible()
        {
            return _windowVisible;
        }

        public void Step(SessionHistory history)
        {
            if (_windowVisible)
            {
                UI.WindowBegin("Report", ref _pose, windowType: UIWin.Body);
                UI.Label("Report");
                UI.HSeparator();

                UI.PushTint(Red);
                if (UI.ButtonRoundAt("Exit Report", Asset.Instance.IconClose, new Vec3(-0.035f, -0.01f, 0), 0.02f))
                {
                    _windowVisible = false;
                    history.resetHistory();
                }
                UI.PopTint();

                history.GetSessionSpans().ForEach(s =>
                {
                    TimeSpan ts = s.GetTimeSpan();
                    UI.Label($"Level {s.Level}: {ts.Minutes}m {ts.Seconds}s");
                });


                List<TimePeriod> touches = history.GetTouchTimePeriods();

                UI.Label($"Total touches: {touches.Count}");

                touches.ForEach(t =>
                {
                    UI.Label($"Start: {t.Begin} End: {t.End}");
                });

                List<TimePeriod> looks = history.GetLookTimePeriods();

                UI.Label($"Total looks: {looks.Count}");

                looks.ForEach(t =>
                {
                    UI.Label($"Start: {t.Begin} End: {t.End}");
                });

                UI.WindowEnd();
            }
        }
    }
}

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

        public void MakeVisible(Pose basePose)
        {
            Vec3 windowPosition = Input.Head.position + Input.Head.Forward * 0.3f;
            Quat lookAt = Quat.LookAt(windowPosition, Input.Head.position);
            _pose = new Pose(windowPosition, lookAt);
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

                UI.SameLine();
                if (UI.Button("Done"))
                    _windowVisible = false;

                UI.HSeparator();

                history.GetLevelHistories().ForEach(s =>
                {
                    TimeSpan ts = s.GetTimeSpan();
                    UI.Label($"Level {s.Level}: {ts.Minutes}m {ts.Seconds}s");
                    UI.Label($"MinDistance: {s.MinDistance}");
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace StereoKitApp
{
    public class SessionHistory
    {
        List<LevelTimeSpan> _sessions = new List<LevelTimeSpan>();
        bool _inProgress;

        public void BeginSession(int level)
        {
            var session = new LevelTimeSpan();
            session.Begin = DateTime.Now;
            session.Level = level;
            _sessions.Add(session);
            
            _inProgress = true;
        }

        public void EndSession()
        {
            if (!_inProgress)
                throw new ApplicationException("No session is in progress!");

            var currentSession = _sessions.Last();
            currentSession.End = DateTime.Now;

            _sessions.RemoveAt(_sessions.Count - 1);
            _sessions.Add(currentSession);
            
            _inProgress = false;
        }

        public List<LevelTimeSpan> GetSessionSpans()
        {
            // Create copy of list
            return _sessions.ToList();
        }

        public TimeSpan GetTotalTime()
        {
            TimeSpan totalTime = new TimeSpan();
            _sessions.ForEach(session =>
            {
                var span = (session.End ?? DateTime.Now) - session.Begin;
                totalTime = totalTime.Add(span);
            });

            return totalTime;
        }

        public TimeSpan GetCurrentLevelTime()
        {
            if (!_inProgress)
                return new TimeSpan();

            return DateTime.Now - _sessions.Last().Begin;
        }

        public bool HasActiveSession()
        {
            return _inProgress;
        }
    }
}

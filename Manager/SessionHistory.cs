using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace StereoKitApp
{
    public class SessionHistory
    {
        private List<LevelTimePeriod> _sessions = new List<LevelTimePeriod>();
        private List<TimePeriod> _touches = new List<TimePeriod>();
        private bool _sessionInProgress;
        private bool _touchingInProgress;

        public void BeginSession(int level)
        {
            if (_sessionInProgress)
                throw new ApplicationException("Session already in progress!");

            var session = new LevelTimePeriod();
            session.Begin = DateTime.Now;
            session.Level = level;
            _sessions.Add(session);
            
            _sessionInProgress = true;
        }

        public void EndSession()
        {
            if (!_sessionInProgress)
                throw new ApplicationException("No session is in progress!");

            var currentSession = _sessions.Last();
            currentSession.End = DateTime.Now;

            _sessions.RemoveAt(_sessions.Count - 1);
            _sessions.Add(currentSession);
            
            _sessionInProgress = false;
        }

        public void resetHistory()
        {
            _sessions.Clear();
            _touches.Clear();
        }

        public List<LevelTimePeriod> GetSessionSpans()
        {
            // Create copy of list
            return _sessions.ToList();
        }

        public TimeSpan GetTotalTime()
        {
            TimeSpan totalTime = new TimeSpan();
            _sessions.ForEach(session =>
            {
                totalTime = totalTime.Add(session.GetTimeSpan());
            });

            return totalTime;
        }

        public TimeSpan GetCurrentLevelTime()
        {
            if (!_sessionInProgress)
                return new TimeSpan();

            return DateTime.Now - _sessions.Last().Begin;
        }

        public bool HasActiveSession()
        {
            return _sessionInProgress;
        }

        public bool Any()
        {
            return _sessions.Count() > 0;
        }

        public void BeginTouchPeriod()
        {
            if (_touchingInProgress)
                throw new ApplicationException("Touch already in progress!");

            TimePeriod period = new TimePeriod();
            period.Begin = DateTime.Now;
            _touches.Add(period);

            _touchingInProgress = true;
        }

        public void EndTouchPeriod()
        {
            if (!_touchingInProgress)
                throw new ApplicationException("No touch in progress!");

            var period = _touches.Last();
            period.End = DateTime.Now;
            _touches.RemoveAt(_touches.Count - 1);
            _touches.Add(period);

            _touchingInProgress = false;
        }

        public List<TimePeriod> GetTouchTimePeriods()
        {
            // create copy of list
            return _touches.ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace StereoKitApp
{
    public class SessionHistory
    {
        private List<LevelTimePeriod> _levelHistories = new List<LevelTimePeriod>();
        private List<TimePeriod> _touches = new List<TimePeriod>();
        private List<TimePeriod> _looks = new List<TimePeriod>();
        private bool _levelInProgress;
        private bool _touchingInProgress;
        private bool _lookingInProgress;

        public void BeginSession(int level)
        {
            if (_levelInProgress)
                throw new ApplicationException("Session already in progress!");

            var levelHistory = new LevelTimePeriod
            {
                Begin = DateTime.Now,
                Level = level
            };

            _levelHistories.Add(levelHistory);
            
            _levelInProgress = true;
        }

        public void EndSession()
        {
            if (!_levelInProgress)
                throw new ApplicationException("No session is in progress!");

            var currentLevel = _levelHistories.Last();
            currentLevel.End = DateTime.Now;

            // Remove old record and replace with new record
            _levelHistories.RemoveAt(_levelHistories.Count - 1);
            _levelHistories.Add(currentLevel);
            
            _levelInProgress = false;
        }

        public void ResetHistory()
        {
            _levelHistories.Clear();
            _touches.Clear();
            _looks.Clear();
        }

        public List<LevelTimePeriod> GetSessionSpans()
        {
            // Create copy of list
            return _levelHistories.ToList();
        }

        public TimeSpan GetTotalTime()
        {
            TimeSpan totalTime = new TimeSpan();
            _levelHistories.ForEach(session =>
            {
                totalTime = totalTime.Add(session.GetTimeSpan());
            });

            return totalTime;
        }

        public TimeSpan GetCurrentLevelTime()
        {
            if (!_levelInProgress)
                return new TimeSpan();

            return DateTime.Now - _levelHistories.Last().Begin;
        }

        public bool HasActiveLevel()
        {
            return _levelInProgress;
        }

        public bool Any()
        {
            return _levelHistories.Count() > 0;
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

        public void BeginLookPeriod()
        {
            if (_lookingInProgress)
                throw new ApplicationException("Look already in progress!");

            TimePeriod period = new TimePeriod();
            period.Begin = DateTime.Now;
            _looks.Add(period);

            _lookingInProgress = true;
        }

        public void EndLookPeriod()
        {
            if (!_lookingInProgress)
                throw new ApplicationException("No look in progress!");

            var period = _looks.Last();
            period.End = DateTime.Now;
            _looks.RemoveAt(_looks.Count - 1);
            _looks.Add(period);

            _lookingInProgress = false;
        }

        public List<TimePeriod> GetLookTimePeriods()
        {
            // create copy of list
            return _looks.ToList();
        }
    }
}

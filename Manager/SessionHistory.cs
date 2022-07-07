using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace StereoKitApp
{
    public class SessionHistory
    {
        private List<LevelHistory> _levelHistories = new List<LevelHistory>();
        private List<TimePeriod> _touches = new List<TimePeriod>();
        private List<TimePeriod> _looks = new List<TimePeriod>();
        private List<TimePeriod> _holds = new List<TimePeriod>();
        private bool _levelInProgress;
        private bool _touchingInProgress;
        private bool _lookingInProgress;
        private bool _holdingInProgress;

        public void BeginLevel(int level)
        {
            if (_levelInProgress)
                throw new ApplicationException("Level already in progress!");

            var levelHistory = new LevelHistory(DateTime.Now, level);

            _levelHistories.Add(levelHistory);
            
            _levelInProgress = true;
        }

        public void EndLevel()
        {
            if (!_levelInProgress)
                throw new ApplicationException("No level is in progress!");

            var currentLevel = _levelHistories.Last();
            currentLevel.End = DateTime.Now;

            // Remove old record and replace with new record
            _levelHistories.RemoveAt(_levelHistories.Count - 1);
            _levelHistories.Add(currentLevel);

            _levelInProgress = false;
        }

        public void UpdateLevelMinDistance(float distance)
        {
            if (!_levelInProgress)
                throw new ApplicationException("No level is in progress!");

            var currentLevel = _levelHistories.Last();
            currentLevel.MinDistance = distance;

            // Remove old record and replace with new record
            _levelHistories.RemoveAt(_levelHistories.Count - 1);
            _levelHistories.Add(currentLevel);
        }

        public void ResetHistory()
        {
            _levelHistories.Clear();
            _touches.Clear();
            _looks.Clear();
            _holds.Clear();

            _touchingInProgress = false;
            _lookingInProgress  = false;
            _holdingInProgress  = false;
        }

        public List<LevelHistory> GetLevelHistories()
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

        public LevelHistory GetCurrentLevelHistory()
        {
            if (!_levelInProgress)
                throw new ApplicationException("No level is in progress!");

            return _levelHistories.Last();
        }

        public bool HasActiveLevel()
        {
            return _levelInProgress;
        }

        public bool Any()
        {
            return _levelHistories.Count() > 0;
        }

        public List<TimePeriod> GetTouchTimePeriods()
        {
            // create copy of list
            return _touches.ToList();
        }

        public List<TimePeriod> GetLookTimePeriods()
        {
            // create copy of list
            return _looks.ToList();
        }

        public List<TimePeriod> GetHoldPeriods()
        {
            // Create copy of list
            return _holds.ToList();
        }

        public List<TimePeriod> GetCurrentLevelTouchTimePeriods()
        {
            LevelHistory current = GetCurrentLevelHistory();
            
            // Filter for the TimePeriods that started during the current level OR carried over from the previous level
            return _touches
                .Where(t => t.Begin > current.Begin || (t.Begin < current.Begin && (t.End ?? DateTime.Now) > current.Begin))
                .ToList();
        }

        public List<TimePeriod> GetCurrentLevelLookTimePeriods()
        {
            LevelHistory current = GetCurrentLevelHistory();

            // Filter for the TimePeriods that started during the current level OR carried over from the previous level
            var currentLooks = _looks
                .Where(l => l.Begin > current.Begin || (l.Begin < current.Begin && (l.End ?? DateTime.Now) > current.Begin))
                .ToList();

            var currentHistory = GetCurrentLevelHistory();

            // Fix-up the begin times if they were carried over from the previous level
            // TODO apply this fix-up to the other TimePeriod getters... or maybe find something more efficient?
            List<TimePeriod> looksFixed = new List<TimePeriod>();
            currentLooks.ForEach(l =>
            {
                if (l.Begin < currentHistory.Begin)
                    l.Begin = currentHistory.Begin;

                looksFixed.Add(l);
            });

            return looksFixed;
        }

        public List<TimePeriod> GetCurrentLevelHoldTimePeriods()
        {
            LevelHistory current = GetCurrentLevelHistory();

            // Filter for the TimePeriods that started during the current level OR carried over from the previous level
            return _holds
                .Where(h => h.Begin > current.Begin || (h.Begin < current.Begin && (h.End ?? DateTime.Now) > current.Begin))
                .ToList();
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

        public void BeginHoldPeriod()
        {
            if (_holdingInProgress)
                throw new ApplicationException("Hold already in progress!");

            TimePeriod period = new TimePeriod();
            period.Begin = DateTime.Now;
            _holds.Add(period);

            _holdingInProgress = true;
        }

        public void EndHoldPeriod()
        {
            if (!_holdingInProgress)
                throw new ApplicationException("No hold in progress!");

            var period = _holds.Last();
            period.End = DateTime.Now;
            _holds.RemoveAt(_holds.Count - 1);
            _holds.Add(period);

            _holdingInProgress = false;
        }
    }
}

﻿using System;
using System.Timers;

namespace NAlex.Helpers
{
    public class DateTimeHelper : IDateTimeHelper
    {
        private const double DayMs = 24.0 * 60.0 * 60.0 * 1000.0;		
        private DateTime _now = DateTime.Now;

        private int _dayInterval;
        private int _day = 1;
        private int _month = 1;
        private int _year = 2017;
        private DateTime _startDate = DateTime.Now;

        public event DayIntervalChangedEventHandler DayIntervalChanged;

        public int DayInterval
        {
            get
            {
                return _dayInterval;
            }
        }

        public DateTime Now
        {
            get
            {
                return new DateTime(_year, _month, _day).AddMilliseconds(
                    (DateTime.Now - _startDate).TotalMilliseconds * DayMs / _dayInterval);
            }
        }

        public void SetDayInterval(int intervalMs)
        {
            if (intervalMs > 0 && intervalMs != _dayInterval)
            {
                DayIntervalEventArgs e = new DayIntervalEventArgs() { OldInterval = _dayInterval, NewInterval = intervalMs };
                DateTime now = Now;
                _year = Now.Year;
                _month = Now.Month;
                _day = Now.Day;
                _dayInterval = intervalMs;
                OnDayIntervalChanged(e);
            }
            
        }

        public DateTimeHelper(int dayIntervalMs)
        {
            _dayInterval = dayIntervalMs;
            
        }

        protected virtual void OnDayIntervalChanged(DayIntervalEventArgs e)
        {
            if (DayIntervalChanged != null)
                DayIntervalChanged(this, e);
        }
    }
}

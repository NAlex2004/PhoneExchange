using System;

namespace NAlex.Helpers
{
    public class DefaultDateTimeHelper: IDateTimeHelper
    {
        private int _interval = 24 * 60 * 60 * 1000;

        public DateTime Now
        {
            get {return DateTime.Now;}
        }

        public int DayInterval
        {
            get { return 24 * 60 * 60 * 1000; }
        }

        public void SetDayInterval(int intervalMs)
        {
            
        }
    }
}
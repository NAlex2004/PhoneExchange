using System;

namespace NAlex.Helpers
{
    public interface IDateTimeHelper
    {
        DateTime Now { get; }
        int DayInterval { get; }

        event DayIntervalChangedEventHandler DayIntervalChanged;

        void SetDayInterval(int intervalMs);
    }
}

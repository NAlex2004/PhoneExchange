using System;

namespace NAlex.Helpers
{
	public interface IDateTimeHelper
	{
		DateTime Now { get; }
		int DayInterval { get; }

		void SetDayInterval(int intervalMs);
	}
}

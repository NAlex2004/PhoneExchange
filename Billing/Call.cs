using System;
using NAlex.APE.Interfaces;

namespace NAlex.Billing
{
	public struct Call
	{
		public DateTime StartDate;
		public TimeSpan Duration;
		public IPortId OtherPortId;
		public bool IsIncomming;
		// стоимость.. можно получить из Tariff, передав один звонок
	}
}

using System;
using NAlex.APE.Interfaces;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing
{
	public class Call
	{
		public Guid CallId;
		public DateTime StartDate;
		public TimeSpan Duration;
		public IPortId SourcePortId;
		public IPortId DestinationPortId;
		public ITariff SourceTariff;
		public ITariff DestinationTariff;
		
//		public IPortId OtherPortId;
//		public bool IsIncomming;

		// стоимость.. можно получить из Tariff, передав один звонок
	}
}

using System;
using NAlex.Billing.Interfaces;
using NAlex.APE.Interfaces;

namespace NAlex.Billing
{
	public class Contract : IContract
	{
		private readonly int _daysToChangeContract;
		
		public DateTime Date { get; protected set; }
		public ITariff Tariff { get; protected set; }
		public DateTime TariffStartDate { get; protected set; }
		public IPort Port { get; protected set; }
		public ContractStates State { get; protected set; }
		public int PaymentDay { get; protected set; }
		
		public virtual bool ChangeTariff(ITariff newTariff)
		{
			bool res = newTariff != null && (DateTime.Now - TariffStartDate).Days >= _daysToChangeContract; 
			if (res)
				Tariff = newTariff;
			return res;
		}

		public Contract(ITariff tariff, IPort port, int paymentDay, int daysToChangeContract)
		{
			if (tariff == null)
				throw new ArgumentNullException(nameof(tariff), "tariff cannot be null.");
			if (port == null)
				throw new ArgumentNullException(nameof(port), "port cannot be null.");
			
			Tariff = tariff;
			Port = port;
			PaymentDay = paymentDay;
			Date = DateTime.Now;
			TariffStartDate = Date;
			State = ContractStates.Active;
			_daysToChangeContract = daysToChangeContract;
		}
	}
}

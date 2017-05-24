using System;
using NAlex.Billing.Interfaces;
using NAlex.APE.Interfaces;

namespace NAlex.Billing
{
	public abstract class Contract
	{
		public DateTime Date { get; protected set; }
		public ITariff Tariff { get; protected set; }
		public DateTime TariffStartDate { get; protected set; }
		public IPort Port { get; protected set; }
		public ContractStates State { get; protected set; }
		public int PaymentDay { get; protected set; }

		public abstract bool ChangeTariff(ITariff newTariff);



	}
}

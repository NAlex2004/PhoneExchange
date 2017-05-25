using System;
using NAlex.APE.Interfaces;

namespace NAlex.Billing.Interfaces
{
	public interface IContract
	{
		DateTime Date { get; }
		ITariff Tariff { get; }
		DateTime TariffStartDate { get; }
		IPort Port { get; }
		ContractStates State { get; }
		int PaymentDay { get; }

		bool ChangeTariff(ITariff newTariff);
	}
}

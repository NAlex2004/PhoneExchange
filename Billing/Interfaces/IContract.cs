using System;
using NAlex.APE.Interfaces;
using NAlex.Billing.Events;

namespace NAlex.Billing.Interfaces
{
	public interface IContract
	{
		DateTime Date { get; }
		ITariff Tariff { get; }
		DateTime TariffStartDate { get; }
		IPort Port { get; }
		ContractStates State { get; set; }
		int PaymentDay { get; }

		event ContractStateChangeEventHandler ContractStateChanging;
		
		bool ChangeTariff(IBilling billing, ITariff newTariff);		
	}
}

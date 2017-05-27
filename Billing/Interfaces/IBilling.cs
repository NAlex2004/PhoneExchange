using System;
using System.Collections.Generic;
using NAlex.APE.Interfaces;
using NAlex.APE;
using NAlex.APE.Event;
using NAlex.Billing;

namespace NAlex.Billing.Interfaces
{
	public interface IBilling
	{
		IEnumerable<ISubscriber> Subscribers { get; }

		IEnumerable<Call> Calls(IContract contract);
		IEnumerable<Payment> Payments(IContract contract);

		bool Pay(IContract contract, double amount);
		ISubscriber Subscribe(string subscriberName, ITariff tariff);
		bool Unsubscribe(ISubscriber subscriber);

		double Cost(IContract contract, Func<Call, bool> condition);
	}
}

using System;
using NAlex.APE.Interfaces;

namespace NAlex.Billing.Interfaces
{
	public interface ISubscriberFactory
	{
		ISubscriber CreateSubscriber(string name, IContract contract, ITerminal terminal);
	}
}

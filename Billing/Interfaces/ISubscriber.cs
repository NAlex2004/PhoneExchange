using System;
using NAlex.APE.Interfaces;

namespace NAlex.Billing.Interfaces
{
	public interface ISubscriber
	{
		string Name { get; }
		IContract Contract { get; }
		ITerminal Terminal { get; }
	}
}

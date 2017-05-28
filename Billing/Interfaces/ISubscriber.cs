using System;
using NAlex.APE.Interfaces;

namespace NAlex.Billing.Interfaces
{
	public interface ISubscriber: IEquatable<ISubscriber>
	{
		string Name { get; }
		IContract Contract { get; }
		ITerminal Terminal { get; }
		IPortId PortId { get; }

		bool ConnectTerminal();
		void DisconnectTerminal();
	}
}

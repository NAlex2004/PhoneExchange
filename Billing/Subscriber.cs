using System;
using NAlex.APE.Interfaces;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing
{
    public class Subscriber: ISubscriber
    {
        public string Name { get; protected set; }
        public IContract Contract { get; protected set; }
        public ITerminal Terminal { get; protected set; }

		public IPortId PortId
		{
			get
			{
				return Contract != null ? Contract.Port.PortId : null;
			}
		}

		public Subscriber(string name, IContract contract, ITerminal terminal)
        {
            Name = name;
            Contract = contract;
            Terminal = terminal;
        }

        public bool Equals(ISubscriber other)
        {
            if (other == null)
                return false;
            
            return string.Equals(Name, other.Name);
        }

		public virtual bool ConnectTerminal()
		{
			return Contract != null ? Contract.Port.Connect(Terminal) : false;
		}

		public virtual void DisconnectTerminal()
		{
			if (Contract != null)
				Contract.Port.Disconnect();
		}
	}
}
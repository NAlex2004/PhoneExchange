using NAlex.APE.Interfaces;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing
{
    public class Subscriber: ISubscriber
    {
        public string Name { get; protected set; }
        public IContract Contract { get; protected set; }
        public ITerminal Terminal { get; protected set; }

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

    }
}
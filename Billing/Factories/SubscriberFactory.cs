using NAlex.APE.Interfaces;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing.Factories
{
    public class SubscriberFactory: ISubscriberFactory
    {
        public ISubscriber CreateSubscriber(string name, IContract contract, ITerminal terminal)
        {
            return new Subscriber(name, contract, terminal);
        }
    }
}
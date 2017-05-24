using NAlex.APE.Interfaces;

namespace NAlex.APE.Factories
{
    public class PePortFactory: IPortFactory
    {
        public IPort CreatePort(IPhoneExchange phoneExchange, IPortId portId)
        {
            return new Port(phoneExchange, portId);
        }
    }
}
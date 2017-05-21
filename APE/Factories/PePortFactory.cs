using NAlex.APE.Interfaces;

namespace NAlex.APE.Factories
{
    public class PePortFactory: IPortFactory
    {
        private IPhoneExchange _phoneExchange;
        
        public PePortFactory(IPhoneExchange phoneExchange)
        {
            _phoneExchange = phoneExchange;
        }
        
        public IPort CreatePort(IPortId portId)
        {
            return new Port(_phoneExchange, portId);
        }
    }
}
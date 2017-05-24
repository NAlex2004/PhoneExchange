namespace NAlex.APE.Interfaces
{
    public interface IPortFactory
    {
		IPort CreatePort(IPhoneExchange phoneExchange, IPortId portId);
    }
}
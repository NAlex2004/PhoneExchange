namespace NAlex.APE.Interfaces
{
    public interface IPortFactory
    {
        IPort CreatePort(IPortId portId);
    }
}
using NAlex.APE.Enums;

namespace NAlex.APE.Interfaces
{
    public interface ITerminal
    {
        bool Connect(IPort port);
        void Disconnect();
        bool StartCall(IPortId portId);
        void EndCall();
        PortStates PortState { get; }
        
        event CallEventHandler CallStarted;
        event CallEventHandler CallEnded;
    }
}
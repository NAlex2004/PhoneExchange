using NAlex.APE.Enums;
using NAlex.APE.Event;

namespace NAlex.APE.Interfaces
{
    public interface ITerminal
    {        
        bool StartCall(IPortId portId); // void
        void EndCall();
        PortStates PortState { get; }
        
        event CallEventHandler CallStarted;
        event CallEventHandler CallEnded;

        void PortStateChanged(object sender, PortEventArgs e);
//        void IncommingCallReceived(object sender, CallEventArgs e);
//        void IncommingCallEnded(object sender, CallEventArgs e);
    }
}
using NAlex.APE.Enums;
using NAlex.APE.Event;

namespace NAlex.APE.Interfaces
{
    public interface ITerminal
    {        
        bool StartCall(IPortId portId);
        void EndCall();
        void AcceptCall();
        
        PortStates PortState { get; }
        
        event CallEventHandler CallStarted;
        event CallEventHandler CallEnded;
        event CallEventHandler CallAccepted;        
        // Для абонента только
        event CallEventHandler CallReceived;                
        
        void PortStateChanged(object sender, PortEventArgs e);
    }
}
using NAlex.APE.Enums;
using NAlex.APE.Event;

namespace NAlex.APE.Interfaces
{
    public interface IPort
    {
        IPortId PortId { get; }
        PortStates PortState { get; }

        // Events for Terminal
        event CallEventHandler CallReceived;
        event CallEventHandler CallEnded;
        // Events for APE (from terminal)
        event CallEventHandler ApeCallStarted;
        event CallEventHandler ApeCallEnded;
        event PortStateEventHandler PortStateChanged;
    }
}
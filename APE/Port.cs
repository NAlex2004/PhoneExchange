using NAlex.APE.Enums;
using NAlex.APE.Event;
using NAlex.APE.Interfaces;

namespace NAlex.APE
{
    public class Port : IPort
    {
        public IPortId PortId { get; protected set; }
        public PortStates PortState { get; protected set; }

        // Events for terminal (from APE)
        public event CallEventHandler CallReceived;
        public event CallEventHandler CallEnded;
        // Events for APE (from terminal)
        public event CallEventHandler ApeCallStarted;
        public event CallEventHandler ApeCallEnded;
        public event PortStateEventHandler PortStateChanged;

        public Port(IPortId portId)
        {
            PortId = portId;
            PortState = PortStates.NotConnected;
        }
        
        public void OutgoingCallStarted(object sender, CallEventArgs e)
        {
            PortState = PortStates.Busy;
            OnApeCallStarted(e);
        }

        public void OutgoingCallEnded(object sender, CallEventArgs e)
        {
            PortState = PortStates.Connected;
            OnApeCallEnded(e);
        }

        public void IncommingCallReceived(object sender, CallEventArgs e)
        {
            PortState = PortStates.Busy;
            OnCallReceived(e);
        }

        public void IncommingCallEnded(object sender, CallEventArgs e)
        {
            PortState = PortStates.Connected;
            OnCallEnded(e);
        }

        protected virtual void OnCallReceived(CallEventArgs e)
        {
            if (CallReceived != null)
                CallReceived(this, e);
        }
        
        protected virtual void OnCallEnded(CallEventArgs e)
        {
            if (CallEnded != null)
                CallEnded(this, e);
        }
        
        protected virtual void OnApeCallStarted(CallEventArgs e)
        {
            if (ApeCallStarted != null)
                ApeCallStarted(this, e);
        }
        
        protected virtual void OnApeCallEnded(CallEventArgs e)
        {
            if (ApeCallEnded != null)
                ApeCallEnded(this, e);
        }        

        protected virtual void OnPortStateChanged(PortSateEventArgs e)
        {
            if (PortStateChanged != null)
                PortStateChanged(this, e);   
        }
    }
}
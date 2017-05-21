using System;
using NAlex.APE.Enums;
using NAlex.APE.Event;
using NAlex.APE.Interfaces;

namespace NAlex.APE
{
    public class Port : IPort
    {
        private ITerminal _terminal;
        
        public IPortId PortId { get; protected set; }
        public PortStates PortState { get; protected set; }

        public Port(IPhoneExchange phoneExchange)
        {
            if (phoneExchange == null)
                throw new ArgumentNullException(nameof(phoneExchange), "phoneExchange cannot be null.");
            phoneExchange.CallStarted += IncommingCallReceived;
            phoneExchange.CallEnded += IncommingCallEnded;
        }
        
        public bool Connect(ITerminal terminal)
        {
            if (terminal == null)
                return false;

            _terminal = terminal;
            _terminal.CallStarted += OutgoingCallStarted;
            _terminal.CallEnded += OutgoingCallEnded;
            PortStateChanged += _terminal.PortStateChanged;

            PortState = PortStates.Connected;
            OnPortStateChanged(new PortEventArgs() {Port = this});
            return true;
        }

        public void Disconnect()
        {
            if (_terminal != null)
            {
                _terminal.CallStarted -= OutgoingCallStarted;
                _terminal.CallEnded -= OutgoingCallEnded;
                PortState = PortStates.NotConnected;
                OnPortStateChanged(new PortEventArgs() {Port = this});
                PortStateChanged -= _terminal.PortStateChanged;
                _terminal = null;
            }
        }

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
        
        protected void OutgoingCallStarted(object sender, CallEventArgs e)
        {
            PortState = PortStates.Busy;
            OnPortStateChanged(new PortEventArgs() {Port = this});
            OnApeCallStarted(e);
        }

        protected void OutgoingCallEnded(object sender, CallEventArgs e)
        {
            PortState = PortStates.Connected;
            OnPortStateChanged(new PortEventArgs() {Port = this});
            OnApeCallEnded(e);
        }

        protected void IncommingCallReceived(object sender, CallEventArgs e)
        {
            PortState = PortStates.Busy;
            OnPortStateChanged(new PortEventArgs() {Port = this});
            OnCallReceived(e);
        }

        protected void IncommingCallEnded(object sender, CallEventArgs e)
        {
            PortState = PortStates.Connected;
            OnPortStateChanged(new PortEventArgs() {Port = this});
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

        protected virtual void OnPortStateChanged(PortEventArgs e)
        {
            if (PortStateChanged != null)
                PortStateChanged(this, e);   
        }
    }
}
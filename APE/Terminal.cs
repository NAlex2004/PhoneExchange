using System;
using System.Diagnostics;
using System.Linq;
using NAlex.APE.Enums;
using NAlex.APE.Event;
using NAlex.APE.Interfaces;

namespace NAlex.APE
{
    public class Terminal : ITerminal
    {
        private IPortId _destPortId;
        private IPort _port;
        private CallEventArgs _call;

        public event CallEventHandler CallStarted;
        public event CallEventHandler CallEnded;        
        public event CallEventHandler CallAccepted;
        // Для абонента только        
        public event CallEventHandler CallReceived;
        
        
        public bool StartCall(IPortId portId)
        {
            if (PortState != PortStates.Connected)
                return false;

            _destPortId = portId;

            _call = new CallEventArgs()
            {
                Date = DateTime.Now,
                DestinationPortId = portId,
                SourcePortId = _port != null ? _port.PortId : null,
                State = CallEventStates.Started
            };

            return OnCallStarted(_call);
        }

        public void EndCall()
        {
            if (PortState != PortStates.Busy || _call == null || _port == null)
                return;

            CallEventArgs eventArgs = (CallEventArgs) _call.Clone();                        
            eventArgs.State = _call.SourcePortId == _port.PortId ? CallEventStates.OutgoingCallFinished : CallEventStates.IncommingCallFinished;
            eventArgs.Date = DateTime.Now;
            _call = null;
            OnCallEnded(eventArgs);                        
        }

        public void AcceptCall()
        {
            if (PortState != PortStates.Busy || _call == null || _port == null)
                return;
            
            _call.State = CallEventStates.Accepted;
            _call.Date = DateTime.Now;
            
            CallEventArgs eventArgs = (CallEventArgs) _call.Clone();                        
            OnCallAccepted(eventArgs);
        }

        public PortStates PortState
        {
            get { return (_port == null ? PortStates.NotConnected : _port.PortState); }
        }

        private void Unsubscribe()
        {
            if (_port != null)
            {
                _port.CallReceived -= IncommingCallReceived;
                _port.CallEnded -= IncommingCallEnded;
                _port.CallAccepted -= OutgoingCallAccepted;
            }
        }

        //----------
        // подписки

        public virtual void PortStateChanged(object sender, PortEventArgs e)
        {            
            IPort port = sender as IPort;
            if (sender != null && e != null && e.Port != null)
            {
                if (_port != null && _port != e.Port)
                    return;
                
                Debug.WriteLine("[Terminal.PortStateChanged] Port: {0}, State: {1}", e.Port.PortId.Value, e.Port.PortState);
                
                if (e.Port.PortState == PortStates.NotConnected)
                {
                    Unsubscribe();
                    _port = null;
                    _destPortId = null;
                }
                else if (e.Port.PortState == PortStates.Connected && _port == null)
                {
                    _port = e.Port;
                    _port.CallReceived += IncommingCallReceived;
                    _port.CallEnded += IncommingCallEnded;
                    _port.CallAccepted += OutgoingCallAccepted;
                }
            }
        }

        protected void IncommingCallReceived(object sender, CallEventArgs e)
        {
            Debug.WriteLine("[Terminal.IncommingCallReceived]");
            Debug.WriteLine(e);
            
            _call = (CallEventArgs) e.Clone();
            OnCallReceived(e);            
        }

        protected void IncommingCallEnded(object sender, CallEventArgs e)
        {
            Debug.WriteLine("[Terminal.IncommingCallEnded]");
            Debug.WriteLine(e);
            
            _call = null;
        }

        protected void OutgoingCallAccepted(object sender, CallEventArgs e)
        {
            Debug.WriteLine("[Terminal.OutgoingCallAccepted]");
            Debug.WriteLine(e);

            _call = (CallEventArgs) e.Clone();
        }
        //----------

        protected virtual bool OnCallStarted(CallEventArgs e)
        {
            if (CallStarted == null)
                return false;

            CallStarted(this, e);

            return (e.State == CallEventStates.IncommingCallFinished 
                    || e.State == CallEventStates.OutgoingCallFinished 
                    || e.State == CallEventStates.Started);
        }

        protected virtual void OnCallEnded(CallEventArgs e)
        {
            if (CallEnded != null)
                CallEnded(this, e);
        }

        protected virtual void OnCallReceived(CallEventArgs e)
        {
            if (CallReceived != null)
                CallReceived(this, e);
        }
        
        protected virtual void OnCallAccepted(CallEventArgs e)
        {
            if (CallAccepted != null)
                CallAccepted(this, e);
        }
    }
}
using System;
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
            OnCallEnded(eventArgs);
            _call = null;            
        }

        public PortStates PortState
        {
            get { return (_port == null ? PortStates.NotConnected : _port.PortState); }
        }

        public event CallEventHandler CallStarted;
        public event CallEventHandler CallEnded;

        private void Unsubscribe()
        {
            if (_port != null)
            {
                _port.CallReceived -= IncommingCallReceived;
                _port.CallEnded -= IncommingCallEnded;
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
                }
            }
        }

        protected void IncommingCallReceived(object sender, CallEventArgs e)
        {
            _call = (CallEventArgs) e.Clone();
        }

        protected void IncommingCallEnded(object sender, CallEventArgs e)
        {
            _call = null;
        }

        //----------

        protected virtual bool OnCallStarted(CallEventArgs e)
        {
            if (CallStarted == null)
                return false;

            CallStarted(this, e);

            return (e.State == CallEventStates.Finished || e.State == CallEventStates.Started);
        }

        protected virtual void OnCallEnded(CallEventArgs e)
        {
            if (CallEnded != null)
                CallEnded(this, e);
        }
    }
}
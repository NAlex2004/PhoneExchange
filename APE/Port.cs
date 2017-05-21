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

        // Events for terminal (from APE)
        public event CallEventHandler CallReceived;
        public event CallEventHandler CallEnded;
        // Events for APE (from terminal)
        public event CallEventHandler ApeCallStarted;
        public event CallEventHandler ApeCallEnded;
        public event PortStateEventHandler PortStateChanged;        
        
        
        public Port(IPhoneExchange phoneExchange, IPortId portId)
        {
            if (phoneExchange == null)
                throw new ArgumentNullException(nameof(phoneExchange), "phoneExchange cannot be null.");

            PortId = portId ?? throw new ArgumentNullException(nameof(portId), "portId cannot be null.");            
            PortState = PortStates.NotConnected;

            phoneExchange.PortAdded += PortAddedToApe;
        }
        
        public bool Connect(ITerminal terminal)
        {
            if (terminal == null)
                return false;

            _terminal = terminal;
            _terminal.CallStarted += TerminalCallStarted;
            _terminal.CallEnded += TerminalCallEnded;
            PortStateChanged += _terminal.PortStateChanged;

            PortState = PortStates.Connected;
            OnPortStateChanged(new PortEventArgs() {Port = this});
            return true;
        }

        public void Disconnect()
        {
            if (_terminal != null)
            {
                _terminal.CallStarted -= TerminalCallStarted;
                _terminal.CallEnded -= TerminalCallEnded;
                PortState = PortStates.NotConnected;
                OnPortStateChanged(new PortEventArgs() {Port = this});
                PortStateChanged -= _terminal.PortStateChanged;
                _terminal = null;
            }
        }

        protected void PortAddedToApe(object sender, PortEventArgs e)
        {
            IPhoneExchange phoneExchange = sender as IPhoneExchange;
            if (phoneExchange != null && e != null && e.Port == this)
            {
                phoneExchange.PortAdded -= PortAddedToApe;
                phoneExchange.PortRemoved += PortRemovedFromApe;
                phoneExchange.CallStarted += IncommingCallReceived;
                phoneExchange.CallEnded += IncommingCallEnded;
            }
        }
        
        protected void PortRemovedFromApe(object sender, PortEventArgs e)
        {
            IPhoneExchange phoneExchange = sender as IPhoneExchange;
            if (phoneExchange != null && e != null && e.Port == this)
            {
                phoneExchange.PortRemoved -= PortRemovedFromApe;
                phoneExchange.CallStarted -= IncommingCallReceived;
                phoneExchange.CallEnded -= IncommingCallEnded;
                phoneExchange.PortRemoved -= PortRemovedFromApe;
            }
        }
        
        
        // Подписки на события от подключенного терминала
        protected void TerminalCallStarted(object sender, CallEventArgs e)
        {
            PortState = PortStates.Busy;
            if (e != null && e.SourcePortId == null)
                e.SourcePortId = PortId;
            OnPortStateChanged(new PortEventArgs() {Port = this});
            OnApeCallStarted(e);
        }

        // Как входящий, так и исходящий
        protected void TerminalCallEnded(object sender, CallEventArgs e)
        {
            PortState = PortStates.Connected;
//            if (e != null && e.SourcePortId == null)
//                e.SourcePortId = PortId;
            OnPortStateChanged(new PortEventArgs() {Port = this});
            OnApeCallEnded(e);
        }
        // -----
        
        // Подписки на события от АТС 
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
        //-----------------
        
        // Сигнал терминалу о звонке
        protected virtual void OnCallReceived(CallEventArgs e)
        {
            if (CallReceived != null)
                CallReceived(this, e);
        }
        
        // Сигнал терминалу об окончании звонка
        protected virtual void OnCallEnded(CallEventArgs e)
        {
            if (CallEnded != null)
                CallEnded(this, e);
        }
        
        // Сигнал для станции о звонке
        protected virtual void OnApeCallStarted(CallEventArgs e)
        {
            if (ApeCallStarted != null)
                ApeCallStarted(this, e);
        }
        
        // Сигнал для станции об окончании звонка
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
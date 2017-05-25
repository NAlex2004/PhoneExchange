using System;
using System.Diagnostics;
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
        public event CallEventHandler CallAccepted;

        // Events for APE (from terminal)
        public event CallEventHandler ApeCallStarted;
        public event CallEventHandler ApeCallEnded;
        public event CallEventHandler ApeCallAccepted;
        public event PortStateEventHandler PortStateChanged;        
        
        
        public Port(IPhoneExchange phoneExchange, IPortId portId)
        {
            if (phoneExchange == null)
                throw new ArgumentNullException(nameof(phoneExchange), "phoneExchange cannot be null.");
			if (portId == null)
				throw new ArgumentNullException(nameof(portId), "portId cannot be null.");

			PortId = portId;
            PortState = PortStates.NotConnected;

            phoneExchange.PortAdded += PortAddedToApe;
        }
        
        public virtual bool Connect(ITerminal terminal)
        {
            if (terminal == null || _terminal != null)
                return false;

            _terminal = terminal;
            _terminal.CallStarted += TerminalCallStarted;
            _terminal.CallEnded += TerminalCallEnded;
            _terminal.CallAccepted += TerminalCallAccepted;
            PortStateChanged += _terminal.PortStateChanged;

            PortState = PortStates.Connected;
            OnPortStateChanged(new PortEventArgs() {Port = this});
            return true;
        }

        public virtual void Disconnect()
        {
            if (_terminal != null)
            {
                PortState = PortStates.NotConnected;
                OnPortStateChanged(new PortEventArgs() {Port = this});
                PortStateChanged -= _terminal.PortStateChanged;
                _terminal.CallStarted -= TerminalCallStarted;
                _terminal.CallEnded -= TerminalCallEnded;
                _terminal.CallAccepted -= TerminalCallAccepted;                
                _terminal = null;
            }
        }

		//-------------------------------------------------------------------------------------------------------------------
		// Подписки
		// Станция	
        protected void PortAddedToApe(object sender, PortEventArgs e)
        {
            IPhoneExchange phoneExchange = sender as IPhoneExchange;
            if (phoneExchange != null && e != null && e.Port == this)
            {
                Debug.WriteLine("[Port.PortAddedToApe] PortId: {0}", e.Port.PortId);
                
                phoneExchange.PortAdded -= PortAddedToApe;
                phoneExchange.PortRemoved += PortRemovedFromApe;
                phoneExchange.CallStarted += IncommingCallReceived;
                phoneExchange.CallEnded += IncommingCallEnded;
                phoneExchange.CallAccepted += OutgoingCallAccepted;
            }
        }
        
        protected void PortRemovedFromApe(object sender, PortEventArgs e)
        {
            IPhoneExchange phoneExchange = sender as IPhoneExchange;
            if (phoneExchange != null && e != null && e.Port == this)
            {
                Debug.WriteLine("[Port.PortRemovedFromApe] PortId: {0}", e.Port.PortId);
                
                phoneExchange.PortRemoved -= PortRemovedFromApe;
                phoneExchange.CallStarted -= IncommingCallReceived;
                phoneExchange.CallEnded -= IncommingCallEnded;
                phoneExchange.PortRemoved -= PortRemovedFromApe;
                phoneExchange.CallAccepted -= OutgoingCallAccepted;
            }
        }
        
        
        // Подписки на события от подключенного терминала
        protected void TerminalCallStarted(object sender, CallEventArgs e)
        {
            Debug.WriteLine("[Port.TerminalCallStarted] PortId: {0}", PortId);
            Debug.WriteLine(e);
            
            PortState = PortStates.Busy;
            if (e != null && e.SourcePortId == null)
                e.SourcePortId = PortId;
            OnPortStateChanged(new PortEventArgs() {Port = this});
            OnApeCallStarted(e);
        }

        // Как входящий, так и исходящий
        protected void TerminalCallEnded(object sender, CallEventArgs e)
        {
            Debug.WriteLine("[Port.TerminalCallEnded] PortId: {0}", PortId);
            Debug.WriteLine(e);
            
            if (PortState != PortStates.NotConnected)
                PortState = PortStates.Connected;
            OnPortStateChanged(new PortEventArgs() {Port = this});
            OnApeCallEnded(e);
        }

        protected void TerminalCallAccepted(object sender, CallEventArgs e)
        {
            Debug.WriteLine("[Port.TerminalCallAccepted] PortId: {0}", PortId);
            Debug.WriteLine(e);
                        
            OnApeCallAccepted(e);
        }
        
        // Подписки на события от АТС 
        protected void IncommingCallReceived(object sender, CallEventArgs e)
        {            
            PortState = PortStates.Busy;
            
            Debug.WriteLine("[Port.IncommingCallReceived] PortId: {0}", PortId);
            Debug.WriteLine(e);
            
            OnPortStateChanged(new PortEventArgs() {Port = this});
            OnCallReceived(e);
        }

        protected void IncommingCallEnded(object sender, CallEventArgs e)
        {            
            if (PortState != PortStates.NotConnected)
                PortState = PortStates.Connected;
            
            Debug.WriteLine("[Port.IncommingCallEnded] PortId: {0}", PortId);
            Debug.WriteLine(e);
            
            OnPortStateChanged(new PortEventArgs() {Port = this});
            OnCallEnded(e);
        }
        
        protected void OutgoingCallAccepted(object sender, CallEventArgs e)
        {            
            Debug.WriteLine("[Port.OutgoingCallAccepted] PortId: {0}", PortId);
            Debug.WriteLine(e);            

			OnCallAccepted(e);
        }
        //-------------------------------------------------------------------------------------------------------------------
        
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

		// Сигнал терминалу, что его звонок принят
		protected virtual void OnCallAccepted(CallEventArgs e)
		{
			if (CallAccepted != null)
				CallAccepted(this, e);
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

        // Сигнал для станции, что звонок принят
        protected virtual void OnApeCallAccepted(CallEventArgs e)
        {
            if (ApeCallAccepted != null)
                ApeCallAccepted(this, e);
        }

        protected virtual void OnPortStateChanged(PortEventArgs e)
        {
            if (PortStateChanged != null)
                PortStateChanged(this, e);   
        }
    }
}
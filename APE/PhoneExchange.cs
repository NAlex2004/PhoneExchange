using System.Collections.Generic;
using System.Linq;
using NAlex.APE.Enums;
using NAlex.APE.Event;
using NAlex.APE.Interfaces;

namespace NAlex.APE
{
    public class PhoneExchange : IPhoneExchange
    {
        private IPortFactory _portFactory;
        private IList<IPort> _ports = new List<IPort>();
        private IList<CallEventArgs> _callLog = new List<CallEventArgs>();
        private IPortId _portId;

        public event CallEventHandler CallStarted;
        public event CallEventHandler CallEnded;
        public event PortStateEventHandler PortAdded;
        public event PortStateEventHandler PortRemoved;

        public IEnumerable<IPort> Ports
        {
            get { return _ports; }
        }

        public IEnumerable<CallEventArgs> CallsLog
        {
            get { return _callLog; }
        }

        public PhoneExchange(IPortFactory portFactory)
        {
            _portFactory = portFactory;
            _portId = _portId.StartValue();
        }

        public IPort CreatePort()
        {
            // На события станции порт подпишится сам при событии PortAdded
            // А на него - в конструкторе
            IPort port = _portFactory.CreatePort(_portId);
            _portId.IncreaseValue();
            port.ApeCallStarted += PortCallStarted;
            port.ApeCallEnded += PortCallEnded;
            _ports.Add(port);
            OnPortAdded(port);
            return port;
        }

        protected virtual void OnPortAdded(IPort port)
        {
            if (PortAdded != null)
                PortAdded(this, new PortEventArgs() {Port = port});
        }

        protected virtual void OnPortRemoved(IPort port)
        {
            if (PortRemoved != null)
                PortRemoved(this, new PortEventArgs() {Port = port});
        }

        protected virtual void OnCall(CallEventHandler handler, CallEventArgs e, IPortId destPortId)
        {
            if (handler != null)
            {
                // Только один нужный порт может быть
                var destPortHandler = handler.GetInvocationList()
                    .Where(d => (d.Target as IPort) != null && ((IPort) d.Target).PortId.Equals(destPortId))
                    .OfType<CallEventHandler>()
                    .FirstOrDefault();

                var invListOthers = handler.GetInvocationList().Where(d => !(d.Target is IPort))
                    .OfType<CallEventHandler>();

                // Вызвать событие только на порту назначения и для всех подписчиков - не портов
                if (destPortHandler != null)
                    destPortHandler(this, e);
                else
                    e.State = CallEventStates.Invalid;

                _callLog.Add(e);

                foreach (var callEventHandler in invListOthers)
                {
                    callEventHandler(this, e);
                }
            }
        }

        // Для портов
        protected virtual void OnCallStarted(CallEventArgs e)
        {
//            e.State = CallEventStates.Started;
            OnCall(CallStarted, e, e.DestinationPortId);
        }

        protected virtual void OnCallEnded(CallEventArgs e)
        {
            OnCall(CallEnded, e, e.DestinationPortId);
        }

        // Подписки на события от портов
        protected virtual void PortCallStarted(object sender, CallEventArgs e)
        {
            var destPort = _ports.FirstOrDefault(p => p.PortId.Equals(e.DestinationPortId));
            if (destPort != null)
            {
                if (destPort.PortState == PortStates.Connected)
                    OnCallStarted(e);
                else
                {
                    e.State = CallEventStates.Missed;
                    OnCall(CallEnded, e, e.SourcePortId);
                }
            }
            else
            {
                // порт назначения не найден, ответ посылается источнику
                e.State = CallEventStates.Invalid;
                OnCall(CallEnded, e, e.SourcePortId);
            }
        }

        protected virtual void PortCallEnded(object sender, CallEventArgs e)
        {
            // разделить входящий и исходящий звонок
            
            var destPort = _ports.FirstOrDefault(p => p.PortId.Equals(e.DestinationPortId));
            if (destPort != null)
            {
                OnCallEnded(e);
            }
        }
    }
}
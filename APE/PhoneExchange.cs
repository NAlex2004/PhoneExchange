using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NAlex.APE.Enums;
using NAlex.APE.Event;
using NAlex.APE.Interfaces;

namespace NAlex.APE
{
	public class PhoneExchange : IPhoneExchange, IBillableExchange
	{
		private IList<IPort> _ports = new List<IPort>();
		private IList<CallEventArgs> _callLog = new List<CallEventArgs>();
		private IPortId _portId;

		public event CallEventHandler CallStarted;
		public event CallEventHandler CallEnded;
		public event CallEventHandler CallAccepted;
		public event PortStateEventHandler PortAdded;
		public event PortStateEventHandler PortRemoved;
		public event CallEventHandler CallPermissionCheck;

		public IEnumerable<IPort> Ports
		{
			get { return _ports; }
		}

		public IEnumerable<CallEventArgs> CallsLog
		{
			get { return _callLog; }
		}

		public PhoneExchange(IPortId startPortId)
		{
			_portId = startPortId;
		}

		public IPort CreatePort(IPortFactory portFactory)
		{
			// На события станции порт подпишится сам при событии PortAdded
			// А на него - в конструкторе
			IPort port = portFactory.CreatePort(_portId);
			_portId = _portId.NextValue();
			port.ApeCallStarted += PortCallStarted;
			port.ApeCallEnded += PortCallEnded;
			port.ApeCallAccepted += PortCallAccepted;
			_ports.Add(port);
			OnPortAdded(port);
			return port;
		}

		protected virtual void OnPortAdded(IPort port)
		{
			if (PortAdded != null)
				PortAdded(this, new PortEventArgs() { Port = port });
		}

		protected virtual void OnPortRemoved(IPort port)
		{
			if (PortRemoved != null)
				PortRemoved(this, new PortEventArgs() { Port = port });
		}

		protected virtual void OnCall(CallEventHandler handler, CallEventArgs e, IPortId destPortId)
		{
			Debug.WriteLine("[PhoneExchange.OnCall]");
			Debug.WriteLine(e);

			if (handler != null)
			{
				// Только один нужный порт может быть
				var destPortHandler = handler.GetInvocationList()
					.Where(d => (d.Target as IPort) != null && ((IPort)d.Target).PortId.Equals(destPortId))
					.OfType<CallEventHandler>()
					.FirstOrDefault();

				var invListOthers = handler.GetInvocationList().Where(d => !(d.Target is IPort))
					.OfType<CallEventHandler>();

				// Вызвать событие только на порту назначения и для всех подписчиков - не портов
				if (destPortHandler != null)
				{
					_callLog.Add(e);
					destPortHandler(this, e);
				}
				else
				{
					e.State = CallEventStates.Invalid;
					_callLog.Add(e);
				}

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

		protected virtual void OnCallAccepted(CallEventArgs e)
		{
			OnCall(CallAccepted, e, e.SourcePortId);
		}

		protected virtual void OnCallPermissionCheck(CallEventArgs e)
		{
			if (CallPermissionCheck != null)
			{
				CallPermissionCheck(this, e);
			}
			else
				e.IsAllowed = true;
		}

		// Подписки на события от портов
		protected virtual void PortCallStarted(object sender, CallEventArgs e)
		{
			OnCallPermissionCheck(e);
			if (!e.IsAllowed)
			{
				e.State = CallEventStates.Denied;
				OnCall(CallEnded, e, e.SourcePortId);
				return;
			}

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
			IPortId destPortId = e.State == CallEventStates.IncommingCallFinished
				? e.SourcePortId
				: e.DestinationPortId;

			var destPort = _ports.FirstOrDefault(p => p.PortId.Equals(destPortId));
			if (destPort != null)
			{
				OnCall(CallEnded, e, destPortId);
			}
		}

		protected virtual void PortCallAccepted(object sender, CallEventArgs e)
		{
			var destPort = _ports.FirstOrDefault(p => p.PortId.Equals(e.SourcePortId));
			if (destPort != null)
				OnCallAccepted(e);
		}
	}
}
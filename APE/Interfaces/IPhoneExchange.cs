using System.Collections.Generic;
using NAlex.APE.Event;

namespace NAlex.APE.Interfaces
{
    public interface IPhoneExchange
    {
        IEnumerable<IPort> Ports { get; }        
        IPort CreatePort();
        bool RemovePort(IPort port);

        event CallEventHandler CallStarted;
        event CallEventHandler CallEnded;
        event CallEventHandler CallAccepted;

        event PortStateEventHandler PortAdded;
        event PortStateEventHandler PortRemoved;
    }
}
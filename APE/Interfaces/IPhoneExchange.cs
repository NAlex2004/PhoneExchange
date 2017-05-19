using System.Collections.Generic;

namespace NAlex.APE.Interfaces
{
    public interface IPhoneExchange
    {
        IEnumerable<IPort> Ports { get; }
        IEnumerable<CallEventArgs> CallsLog { get; }
        IPort CreatePort();

        event CallEventHandler CallStarted;
        event CallEventHandler CallEnded;
    }
}
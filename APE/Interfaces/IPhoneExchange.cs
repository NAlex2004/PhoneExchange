using System.Collections.Generic;

namespace NAlex.APE.Interfaces
{
    public interface IPhoneExchange
    {
        IEnumerable<IPort> Ports { get; }
        IEnumerable<CallEventArgs> CallsLog { get; }

        event CallEventHandler CallStarted;
        event CallEventHandler CallEnded;
    }
}
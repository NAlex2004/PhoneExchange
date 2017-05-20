using System;
using NAlex.APE.Enums;
using NAlex.APE.Interfaces;

namespace NAlex.APE.Event
{
    public class CallEventArgs: EventArgs
    {
        public IPortId SourcePortId { get; set; }
        public IPortId DestinationPortId { get; set; }
        public DateTime Date { get; set; }
        public CallEventStates State { get; set; }
    }
}
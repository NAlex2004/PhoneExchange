using System;
using System.Text;
using NAlex.APE.Enums;
using NAlex.APE.Interfaces;

namespace NAlex.APE.Event
{
    public class CallEventArgs: EventArgs, ICloneable
    {
        public IPortId SourcePortId { get; set; }
        public IPortId DestinationPortId { get; set; }
        public DateTime Date { get; set; }
        public CallEventStates State { get; set; }
		public bool IsAllowed { get; set; }
        
        public object Clone()
        {
            return new CallEventArgs()
            {
                Date = this.Date,
                SourcePortId = this.SourcePortId,
                DestinationPortId = this.DestinationPortId,
                State = this.State
            };
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\t[CallEventArgs]");
            sb.AppendLine(string.Format("SourcePortId: {0}", SourcePortId));
            sb.AppendLine(string.Format("DestinationPortId: {0}", DestinationPortId));
            sb.AppendLine(string.Format("Date: {0}", Date));
            sb.AppendLine(string.Format("State: {0}", State));
            return sb.ToString();
        }
    }
}
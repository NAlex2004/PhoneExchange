using System;
using System.Text;
using NAlex.APE.Interfaces;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing
{
	public class Call
	{
		public Guid CallId;
		public DateTime StartDate;
		public TimeSpan Duration;
		public IPortId SourcePortId;
		public IPortId DestinationPortId;
		public ITariff SourceTariff;
		public ITariff DestinationTariff;
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("[CALL]");
			sb.AppendLine(string.Format("CallId: {0}", CallId));
			sb.AppendLine(string.Format("SourcePortId: {0}", SourcePortId));
			sb.AppendLine(string.Format("DestinationPortId: {0}", DestinationPortId));
			sb.AppendLine(string.Format("Date: {0}", StartDate));
			sb.AppendLine(string.Format("Duration: {0}", Duration));
			sb.AppendLine();
			return sb.ToString();
		}
	}
}

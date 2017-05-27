using System;
using System.Collections.Generic;
using NAlex.APE.Event;

namespace NAlex.APE.Interfaces
{
	public interface IBillableExchange
	{
//		IEnumerable<CallEventArgs> CallsLog { get; }
		event CallEventHandler CallLog;
		event CallEventHandler CallPermissionCheck;
	}
}

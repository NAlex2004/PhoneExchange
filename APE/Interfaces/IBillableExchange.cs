using System;
using System.Collections.Generic;
using NAlex.APE.Event;

namespace NAlex.APE
{
	public interface IBillableExchange
	{
		IEnumerable<CallEventArgs> CallsLog { get; }
		event CallEventHandler CallPermissionCheck;
	}
}

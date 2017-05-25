using System;
using System.Collections.Generic;
using NAlex.APE.Interfaces;
using NAlex.APE;
using NAlex.APE.Event;
using NAlex.Billing;

namespace NAlex.Billing.Interfaces
{
	public interface IBilling
	{
		//IBillableExchange PhoneExchange { get; }
		IEnumerable<Payment> Payments { get; }
		IEnumerable<Contract> Contracts { get; }

		IEnumerable<Call> Calls(Contract contract);
	}
}

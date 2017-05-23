using System;
using System.Collections.Generic;

namespace NAlex.Billing.Interfaces
{
	public interface ITariff
	{
		double Cost { get; }
		string Description { get; }
		double TotalAmount(IEnumerable<Call> calls);
	}
}

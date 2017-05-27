using System;
using System.Collections.Generic;

namespace NAlex.Billing.Interfaces
{
	public interface ITariff
	{
		double Fee { get; }
		double Cost { get; }
		string Description { get; }
		double TotalAmount(IEnumerable<Call> calls, int days = 0);
	}
}

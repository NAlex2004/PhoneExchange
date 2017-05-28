using System;
using System.Collections.Generic;
using NAlex.APE.Interfaces;

namespace NAlex.Billing.Interfaces
{
	public interface ITariff
	{
		double Fee { get; }
		double Cost { get; }
		string Description { get; }

		double TotalFee(int days);
		double CallCost(IPortId portId, Call call);
	}
}

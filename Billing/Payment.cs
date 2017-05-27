using System;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing
{
	public struct Payment
	{
		public DateTime Date;
		public IContract Contract;
		public double Amount;
	}
}

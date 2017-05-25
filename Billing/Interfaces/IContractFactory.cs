using System;
using NAlex.APE.Interfaces;

namespace NAlex.Billing.Interfaces
{
	public interface IContractFactory
	{
		IContract CreateContract(ITariff tariff, IPort port);
	}
}

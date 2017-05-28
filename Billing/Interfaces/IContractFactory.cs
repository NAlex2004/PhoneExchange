using System;
using NAlex.APE.Interfaces;
using NAlex.Helpers;

namespace NAlex.Billing.Interfaces
{
	public interface IContractFactory
	{
		IContract CreateContract(ITariff tariff, IPort port);
	}
}

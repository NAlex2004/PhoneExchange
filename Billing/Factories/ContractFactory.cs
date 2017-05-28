using NAlex.APE.Interfaces;
using NAlex.Billing.Interfaces;
using NAlex.Helpers;

namespace NAlex.Billing.Factories
{
	public class ContractFactory : IContractFactory
	{
		private IDateTimeHelper _dtHelper;

		public ContractFactory(IDateTimeHelper dtHelper = null)
		{
			_dtHelper = dtHelper;
		}

		public IContract CreateContract(ITariff tariff, IPort port)
		{
			return new Contract(tariff, port, 25, 30, _dtHelper);
		}
	}
}
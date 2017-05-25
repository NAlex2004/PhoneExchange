using NAlex.APE.Interfaces;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing.Factories
{
    public class ContractFactory: IContractFactory
    {
        public IContract CreateContract(ITariff tariff, IPort port)
        {
            return new Contract(tariff, port, 25, 30);
        }
    }
}
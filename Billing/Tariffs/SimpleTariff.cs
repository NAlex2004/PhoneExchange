using System.Collections.Generic;

namespace NAlex.Billing.Tariffs
{
    public class SimpleTariff: BaseTariff
    {
        public SimpleTariff()
        {
            Fee = 3.25;
            Cost = 0.03;
            Description = "Абонентская плата 3.25 р./30 дней, Исходящий звонок - 3 коп./минута.";                       
        }

        public override double TotalAmount(IEnumerable<Call> calls, int days = 0)
        {
            return base.TotalAmount(calls, days) + days * Fee / 30;
        }
    }
}
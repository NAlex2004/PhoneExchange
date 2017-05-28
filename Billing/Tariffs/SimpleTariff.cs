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
    }
}
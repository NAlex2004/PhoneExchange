using System.Collections.Generic;
using System.Linq;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing.Tariffs
{
    public class BaseTariff: ITariff
    {
        public double Fee { get; protected set; }
        public double Cost { get; protected set; }
        public string Description { get; protected set; }
        
        public virtual double TotalAmount(IEnumerable<Call> calls, int days = 0)
        {
            if (calls == null)
                return 0;

            return calls.Where(c => !c.IsIncomming).Sum(c => c.Duration.Minutes) * Cost;
        }

        public BaseTariff()
        {
            Fee = 0;
            Cost = 0.2;
            Description = "Без абонентской платы. 20 к. за минуту исходящих. Входящие бесплатны.";
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
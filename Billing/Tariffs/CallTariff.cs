using System;
using System.Collections.Generic;
using System.Linq;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing.Tariffs
{
    public class CallTariff: ITariff
    {
        public double IncomingCost { get; protected set; }
        
        public double Fee { get; protected set; }
        public double Cost { get; protected set; }
        public string Description { get; protected set; }
        
        public virtual double TotalAmount(IEnumerable<Call> calls, int days = 0)
        {
            if (calls == null)
                return 0;

            return calls.Sum(c => c.IsIncomming ? IncomingCost : Cost) + days * Fee / 30;
        }

        public CallTariff()
        {
            IncomingCost = 0.05;
            Cost = 0.5;
            Fee = 2;
            Description = "Абонентская плата: 2 р./30 дней. Входящий звонок - 5 коп., исходящий - 50 коп.";
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using NAlex.APE.Interfaces;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing.Tariffs
{
    public class BaseTariff: ITariff
    {
        public double Fee { get; protected set; }
        public double Cost { get; protected set; }
        public string Description { get; protected set; }
        
        public virtual double TotalFee(int days)
        {
            return days * Fee / 30;
        }

        public virtual double CallCost(IPortId portId, Call call)
        {
            return call.SourcePortId.Equals(portId) ? call.Duration.TotalMinutes * Cost : 0;
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
using System;
using System.Collections.Generic;
using System.Linq;
using NAlex.APE.Interfaces;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing.Tariffs
{
    public class CallTariff: ITariff
    {
        public double IncomingCost { get; protected set; }
        
        public double Fee { get; protected set; }
        public double Cost { get; protected set; }
        public string Description { get; protected set; }
        
        public virtual double TotalFee(int days)
        {
            return days * Fee / 30;
        }

        public virtual double CallCost(IPortId portId, Call call)
        {
            return call.SourcePortId.Equals(portId) ? Cost : (call.DestinationPortId.Equals(portId) ? IncomingCost : 0);
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
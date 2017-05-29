using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using NAlex.APE;
using NAlex.APE.Event;
using NAlex.APE.Factories;
using NAlex.APE.Interfaces;
using NAlex.APE.Enums;
using NAlex.Billing;
using NAlex.Billing.Factories;
using NAlex.Billing.Interfaces;
using NAlex.Helpers;
using NAlex.Billing.Tariffs;

namespace PhoneDemo
{
    internal class Program
    {		
        public static void Main(string[] args)
        {			
            using (PhoneOperator demoOperator = new PhoneOperator())
            {
                demoOperator.CreateSomeSubscribers();
                demoOperator.ConnectTerminals();
                demoOperator.MakeSomeCalls();

                demoOperator.WriteCalls(c => c.SourcePortId.Equals(demoOperator.Billing.Subscribers.ElementAt(0).PortId)
                    || c.DestinationPortId.Equals(demoOperator.Billing.Subscribers.ElementAt(0).PortId));

                demoOperator.WriteBalance();
                demoOperator.PassSomeDays(33);
                demoOperator.WriteBalance();
                demoOperator.MakeSomePayments();
                demoOperator.WriteBalance();

                demoOperator.WriteCalls(demoOperator.Billing.Subscribers.ElementAt(1).Contract, c => true);

                demoOperator.DisconnectTerminals();
            }

            Console.ReadKey();
        }
    }
}
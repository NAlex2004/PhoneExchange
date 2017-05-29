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

                ISubscriber subscr1 = demoOperator.Billing.Subscribers.ElementAt(0);
                ISubscriber subscr2;

                Console.WriteLine("\n\n-------- Calls for {0} (PortId = {1})------",
                    subscr1.Name, subscr1.PortId);                
                demoOperator.WriteCalls(c => c.SourcePortId.Equals(subscr1.PortId) || c.DestinationPortId.Equals(subscr1.PortId));

                demoOperator.WriteBalance();

                demoOperator.PassSomeDays(33);

                demoOperator.WriteBalance();                
                demoOperator.MakeSomePayments();
                demoOperator.WriteBalance();

                demoOperator.MakeSomeCalls();
                demoOperator.MakeSomeCalls();

                subscr1 = demoOperator.Billing.Subscribers.ElementAt(3);

                Console.WriteLine("\n\n-------- Calls for {0} (PortId = {1})------",
                    subscr1.Name, subscr1.PortId);

                demoOperator.WriteCalls(subscr1.Contract, c => true);

                subscr1 = demoOperator.Billing.Subscribers.ElementAt(3);
                subscr2 = demoOperator.Billing.Subscribers.ElementAt(1);

                Console.WriteLine("\n\n-------- Calls for {0} (PortId = {1}) to {2} (PortId = {3})------",
                    subscr1.Name, subscr1.PortId,
                    subscr2.Name, subscr2.PortId);

                demoOperator.Billing.Calls(subscr1.Contract).Where(c => c.DestinationPortId.Equals(subscr2.PortId))
                    .OrderBy(c => c.Duration).ToList().ForEach(c => Console.WriteLine(c));

                demoOperator.DisconnectTerminals();
            }

            Console.ReadKey();
        }
    }
}
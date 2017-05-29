﻿using System;
using System.Linq;
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
using System.Threading;

namespace PhoneDemo
{
    public class PhoneOperator: IDisposable
    {
        Random rnd = new Random();
        IDateTimeHelper dtHelper;
        IBillableExchange ape;
        public IBilling Billing { get; protected set; }

        public PhoneOperator(int dayInterval = 100000)
        {
            dtHelper = new DateTimeHelper(dayInterval);

            IPortFactory pFactory = new PePortFactory();
            ape = new PhoneExchange(pFactory, (new IntId()).StartValue());
            Billing = new Billing(ape, new ContractFactory(dtHelper), new SubscriberFactory(), dtHelper);
        }

        public void CreateSomeSubscribers()
        {
            Billing.Subscribe("John", new BaseTariff());
            Billing.Subscribe("Jack", new CallTariff());
            Billing.Subscribe("Mary", new SimpleTariff());
            Billing.Subscribe("Alex", new SimpleTariff());            
        }

        public void ConnectTerminals()
        {
            foreach (var subscriber in Billing.Subscribers)
            {
                subscriber.ConnectTerminal();
                subscriber.Terminal.CallReceived += (sender, e) => (sender as ITerminal).AcceptCall();
                subscriber.Contract.Port.CallEnded += (sender, e) =>
                    {
                        Console.WriteLine("\tCall from {0} to {1} ending state: {2}", e.SourcePortId, e.DestinationPortId, e.State);
                    };
            }
        }

        public void DisconnectTerminals()
        {
            foreach (var subscriber in Billing.Subscribers)
                subscriber.DisconnectTerminal();
        }

        public void MakeSomeCalls()
        {            
            Billing.Subscribers.ElementAt(0).Terminal.StartCall(Billing.Subscribers.ElementAt(1).PortId);
            Billing.Subscribers.ElementAt(2).Terminal.StartCall(Billing.Subscribers.ElementAt(3).PortId);
            Thread.Sleep(rnd.Next(30) + 200);
            Billing.Subscribers.ElementAt(2).Terminal.EndCall();
            Thread.Sleep(rnd.Next(20) + 30);
            Billing.Subscribers.ElementAt(1).Terminal.EndCall();

            Billing.Subscribers.ElementAt(3).Terminal.StartCall(Billing.Subscribers.ElementAt(0).PortId);
            Billing.Subscribers.ElementAt(2).Terminal.StartCall(Billing.Subscribers.ElementAt(1).PortId);
            Thread.Sleep(rnd.Next(60) + 200);
            Billing.Subscribers.ElementAt(2).Terminal.EndCall();
            Thread.Sleep(rnd.Next(40) + 40);
            Billing.Subscribers.ElementAt(0).Terminal.EndCall();

            Billing.Subscribers.ElementAt(1).Terminal.StartCall(Billing.Subscribers.ElementAt(2).PortId);
            Billing.Subscribers.ElementAt(0).Terminal.StartCall(Billing.Subscribers.ElementAt(3).PortId);
            Thread.Sleep(rnd.Next(30) + 200);
            Billing.Subscribers.ElementAt(2).Terminal.EndCall();
            Thread.Sleep(rnd.Next(20) + 40);
            Billing.Subscribers.ElementAt(0).Terminal.EndCall();

            Billing.Subscribers.ElementAt(3).Terminal.StartCall(Billing.Subscribers.ElementAt(2).PortId);
            Thread.Sleep(rnd.Next(50) + 200);
            Billing.Subscribers.ElementAt(3).Terminal.EndCall();

            Billing.Subscribers.ElementAt(3).Terminal.StartCall(Billing.Subscribers.ElementAt(0).PortId);
            Thread.Sleep(rnd.Next(60) + 200);
            Billing.Subscribers.ElementAt(3).Terminal.EndCall();

            Billing.Subscribers.ElementAt(3).Terminal.StartCall(Billing.Subscribers.ElementAt(1).PortId);
            Thread.Sleep(rnd.Next(10) + 200);
            Billing.Subscribers.ElementAt(3).Terminal.EndCall();
        }

        public void PassSomeDays(int days)
        {
            if (days > 0)
            {
                int dayInterval = dtHelper.DayInterval;
                dtHelper.SetDayInterval(200);
                Thread.Sleep(days * 200);
                dtHelper.SetDayInterval(dayInterval);
                Console.WriteLine();
                Console.WriteLine("... Passed {0} days ...", days);
                Console.WriteLine();
            }
        }

        public void MakeSomePayments()
        {
            Billing.Pay(Billing.Subscribers.ElementAt(0).Contract, 1);
            Billing.Pay(Billing.Subscribers.ElementAt(1).Contract, 2);
            Billing.Pay(Billing.Subscribers.ElementAt(3).Contract, 4);

            Console.WriteLine("-------- Make some payments.. ----------");
        }

        public void WriteBalance(ISubscriber subscr = null)
        {
            DateTime now = dtHelper.Now;
            Console.WriteLine();
            Console.WriteLine("----------- Balance on date: {0} -------------", now);
            Console.WriteLine();
            var subscribers = subscr == null ? Billing.Subscribers : Billing.Subscribers.Where(s => s.Equals(subscr));
            foreach (var subscriber in subscribers)
            {
                Console.WriteLine("Subscriber: {0}", subscriber.Name);
                Console.WriteLine("Tariff: {0}", subscriber.Contract.Tariff);
                Console.WriteLine("Balance: {0}", Billing.Balance(subscriber.Contract));
                Console.WriteLine("Contract state: {0}", subscriber.Contract.State);
                Console.WriteLine();
            }
        }

        public void WriteCalls(Func<Call, bool> condition)
        {
            Console.WriteLine();
            Console.WriteLine("--------------- Calls: ----------------");
            Console.WriteLine();
            Billing.Calls().Where(condition).ToList()
                .ForEach(c => 
                {
                    Console.WriteLine(c); 
                });
        }

        public void WriteCalls(IContract contract, Func<Call, bool> condition)
        {
            Console.WriteLine();
            Console.WriteLine("--------------- Calls: ----------------");
            Console.WriteLine();
            Billing.Calls(contract).Where(condition).ToList()
                .ForEach(c =>
                {
                    Console.WriteLine(c);
                    Console.WriteLine("Cost: {0}", contract.Tariff.CallCost(contract.Port.PortId, c));
                    Console.WriteLine();
                });
        }

        public void ChangeTariff(ISubscriber subscr, ITariff tariff)
        {
            Console.WriteLine("------------- Change tariff for {0} --------------", subscr.Name);
            Console.WriteLine("New tariff: {0}", tariff);
            Console.WriteLine("Success: {0}", subscr.Contract.ChangeTariff(Billing, tariff));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Billing.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);		
        }
        #endregion
    }
}

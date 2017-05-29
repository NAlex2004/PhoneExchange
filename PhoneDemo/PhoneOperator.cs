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
            Thread.Sleep(rnd.Next(30) + 20);
            Billing.Subscribers.ElementAt(2).Terminal.EndCall();
            Thread.Sleep(rnd.Next(20) + 15);
            Billing.Subscribers.ElementAt(1).Terminal.EndCall();

            Billing.Subscribers.ElementAt(3).Terminal.StartCall(Billing.Subscribers.ElementAt(0).PortId);
            Billing.Subscribers.ElementAt(2).Terminal.StartCall(Billing.Subscribers.ElementAt(1).PortId);
            Thread.Sleep(rnd.Next(60) + 20);
            Billing.Subscribers.ElementAt(2).Terminal.EndCall();
            Thread.Sleep(rnd.Next(40) + 15);
            Billing.Subscribers.ElementAt(0).Terminal.EndCall();

            Billing.Subscribers.ElementAt(1).Terminal.StartCall(Billing.Subscribers.ElementAt(2).PortId);
            Billing.Subscribers.ElementAt(0).Terminal.StartCall(Billing.Subscribers.ElementAt(3).PortId);
            Thread.Sleep(rnd.Next(30) + 30);
            Billing.Subscribers.ElementAt(2).Terminal.EndCall();
            Thread.Sleep(rnd.Next(20) + 40);
            Billing.Subscribers.ElementAt(1).Terminal.EndCall();
        }

        public void PassSomeDays(int days)
        {
            if (days > 0)
            {
                int dayInterval = dtHelper.DayInterval;
                dtHelper.SetDayInterval(100);
                Thread.Sleep(days * 100);
                dtHelper.SetDayInterval(dayInterval);
            }
        }

        public void MakeSomePayments()
        {
            
        }

        public void WriteSomeStatistics()
        {
            DateTime now = dtHelper.Now;
            Console.WriteLine("Balance on date: {0}", now);
            Console.WriteLine();
            foreach (var subscriber in Billing.Subscribers)
            {
                Console.WriteLine("Balance for subscriber {0}: {1}", subscriber.Name, Billing.Balance(subscriber.Contract, now));
            }
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

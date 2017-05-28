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
				
			}


			IDateTimeHelper dtHelper = new DateTimeHelper(100000);
						
			IPortFactory pFactory = new PePortFactory();
			IBillableExchange ape = new PhoneExchange(pFactory, (new IntId()).StartValue());									
			IBilling billing = new Billing(ape, new ContractFactory(dtHelper), new SubscriberFactory(), dtHelper);

			ISubscriber john = billing.Subscribe("John", new BaseTariff());
			ISubscriber jack = billing.Subscribe("Jack", new CallTariff());
			ISubscriber mary = billing.Subscribe("Mary", new CallTariff());

			john.ConnectTerminal();
			jack.ConnectTerminal();
			mary.ConnectTerminal();

			john.Terminal.CallReceived += (sender, e) => { (sender as ITerminal).AcceptCall(); };
			jack.Terminal.CallReceived += (sender, e) => { (sender as ITerminal).AcceptCall(); };				
			mary.Terminal.CallReceived += (sender, e) => { (sender as ITerminal).AcceptCall(); };				

			john.Terminal.StartCall(jack.PortId);
			mary.Terminal.StartCall(john.PortId);

			Thread.Sleep(100);

			jack.Terminal.EndCall();
			mary.Terminal.StartCall(john.PortId);

			Thread.Sleep(100);

			mary.Terminal.EndCall();

			john.Terminal.StartCall(mary.PortId);

			Thread.Sleep(100);

			john.DisconnectTerminal();

			dtHelper.SetDayInterval(1000);
			Thread.Sleep(2000);

			billing.Calls(john.Contract).ToList().ForEach(c => Console.WriteLine(c));
			Console.WriteLine(billing.Balance(jack.Contract, dtHelper.Now));
		}
	}
}
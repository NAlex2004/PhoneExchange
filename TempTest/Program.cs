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

namespace TempTest
{
	internal class Program
	{
		static void CallReceived(object sender, CallEventArgs e)
		{
			ITerminal terminal = sender as ITerminal;
			Thread.Sleep(1500);
			//            terminal.EndCall();
			terminal.AcceptCall();
		}

		public static void Main(string[] args)
		{
			IPhoneExchange ape = new PhoneExchange((new IntId()).StartValue());
			IPortFactory pFactory = new PePortFactory(ape);

			IPort p1 = ape.CreatePort(pFactory);
			IPort p2 = ape.CreatePort(pFactory);
			IPort p3 = ape.CreatePort(pFactory);

			ITerminal t1 = new Terminal();
			ITerminal t2 = new Terminal();
			ITerminal t3 = new Terminal();

			p1.Connect(t1);
			p2.Connect(t2);
			p2.Connect(t3);
			p3.Connect(t3);
			t2.CallReceived += CallReceived;
			t1.StartCall(p2.PortId);
			t2.StartCall(p3.PortId);
			t3.StartCall(p1.PortId);
			Thread.Sleep(2000);
			t1.EndCall();

			t1.CallReceived += CallReceived;
			t2.StartCall(p1.PortId);
			Thread.Sleep(1000);


			t1.StartCall(p3.PortId);
			p1.Disconnect();
			p2.Disconnect();
			p3.Disconnect();
			t2.CallReceived -= CallReceived;
			t1.CallReceived -= CallReceived;

			var calls = (ape as IBillableExchange).CallsLog
									  .Where(c =>
											 (c.SourcePortId.Equals(p1.PortId) || c.DestinationPortId.Equals(p1.PortId))
											 &&
											 (c.State == CallEventStates.Accepted || c.State == CallEventStates.IncommingCallFinished || c.State == CallEventStates.OutgoingCallFinished))
									  //.OrderBy(e => e.Date)
									  .GroupBy(g => g.CallId, a => new { a.Date, a.SourcePortId, a.DestinationPortId, a.State, a.IsAllowed });
			//.ToList()
			//.ForEach(e => Console.WriteLine("{0}\n", e));

			foreach (var g in calls)
			{
				Call call;
				call.StartDate = g.Where(c => c.State == CallEventStates.Accepted).Select(c => c.Date).FirstOrDefault();
				if (call.StartDate != default(DateTime))
				{
					var ce = g.Where(c => c.State != CallEventStates.Accepted).FirstOrDefault();
					call.IsIncomming = ce.State == CallEventStates.IncommingCallFinished;
					call.OtherPortId = call.IsIncomming ? ce.SourcePortId : ce.DestinationPortId;
					call.Duration = ce.Date - call.StartDate;

					Console.WriteLine("Call for port {0}", p1.PortId);
					Console.WriteLine("IsIncomming: {0}", call.IsIncomming);
					Console.WriteLine("OtherPortId: {0}", call.OtherPortId);
					Console.WriteLine("Duration: {0}", call.Duration);
				}

			}
			//            ape.CallsLog.Where(e => e.SourcePortId.Equals(new IntId() {Id = 1}) && e.State == )
		}
	}
}
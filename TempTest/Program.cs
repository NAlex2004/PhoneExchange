using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using NAlex.APE;
using NAlex.APE.Event;
using NAlex.APE.Factories;
using NAlex.APE.Interfaces;

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
            
            t1.EndCall();
            t1.StartCall(p3.PortId);
            p1.Disconnect();
            p2.Disconnect();
            p3.Disconnect();
            t2.CallReceived -= CallReceived;
         
            ape.CallsLog.OrderBy(e => e.Date).ToList().ForEach(e => Console.WriteLine("{0}\n", e));
//            ape.CallsLog.Where(e => e.SourcePortId.Equals(new IntId() {Id = 1}) && e.State == )
        }
    }
}
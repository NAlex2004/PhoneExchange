using System;
using System.Collections.Generic;
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
            terminal.AcceptCall();
        }
        
        public static void Main(string[] args)
        {
            IPhoneExchange ape = new PhoneExchange((new IntId()).StartValue());
            IPortFactory pFactory = new PePortFactory(ape);

            IPort p1 = ape.CreatePort(pFactory);
            IPort p2 = ape.CreatePort(pFactory);
            
            ITerminal t1 = new Terminal();
            ITerminal t2 = new Terminal();

            p1.Connect(t1);
            p2.Connect(t2);
            t2.CallReceived += CallReceived;
            t1.StartCall(p2.PortId);
            
            t1.EndCall();
            
            p1.Disconnect();
            p2.Disconnect();
            t2.CallReceived -= CallReceived;
        }
    }
}
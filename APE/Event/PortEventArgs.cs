using System;
using NAlex.APE.Enums;
using NAlex.APE.Interfaces;

namespace NAlex.APE.Event
{
    public class PortEventArgs: EventArgs
    {
        public IPort Port { get; set; }
//        public PortStates PortState { get; set; }
    }
}
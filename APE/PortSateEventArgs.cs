using System;
using NAlex.APE.Enums;

namespace NAlex.APE
{
    public class PortSateEventArgs: EventArgs
    {
        public PortStates PortState { get; set; }
    }
}
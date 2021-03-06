﻿using NAlex.APE.Enums;
using NAlex.APE.Event;

namespace NAlex.APE.Interfaces
{
    public interface IPort
    {
        IPortId PortId { get; }
        PortStates PortState { get; }

        bool Connect(ITerminal terminal);
        void Disconnect();
        // Events for Terminal
        event CallEventHandler CallReceived;
        event CallEventHandler CallEnded;
        event CallEventHandler CallAccepted;
        // Events for APE (from terminal)
        event CallEventHandler ApeCallStarted;
        event CallEventHandler ApeCallEnded;
        event CallEventHandler ApeCallAccepted;
        event PortStateEventHandler PortStateChanged;
    }
}
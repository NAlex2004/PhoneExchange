using System;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing.Events
{
    public class ContractStateChangeEventArgs: EventArgs
    {
        public IContract Contract;
        public ContractStates OldState;
        public ContractStates NewState;
        public bool ChangeAllowed;
    }
}
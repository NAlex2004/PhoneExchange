using System;
using System.Collections.Generic;
using System.Linq;
using NAlex.APE;
using NAlex.APE.Enums;
using NAlex.APE.Event;
using NAlex.APE.Interfaces;
using NAlex.Billing.Events;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing
{
    public class Billing: IBilling
    {
        private bool _allowContractStateChange = false;
        
        private IBillableExchange _phoneExchange;
        private IContractFactory _contractFactory;
        private ISubscriberFactory _subscriberFactory;
        
        private IList<CallEventArgs> _callLog = new List<CallEventArgs>();
        private IList<ISubscriber> _subscribers = new List<ISubscriber>();
        private IList<Payment> _payments = new List<Payment>();

        public IEnumerable<ISubscriber> Subscribers
        {
            get { return _subscribers; }
        }
        
        public IEnumerable<Call> Calls(IContract contract)
        {
            return _callLog
                .Where(c =>
                    (c.SourcePortId.Equals(contract.Port.PortId) || c.DestinationPortId.Equals(contract.Port.PortId))
                    &&
                    (c.State == CallEventStates.Accepted || c.State == CallEventStates.IncommingCallFinished || c.State == CallEventStates.OutgoingCallFinished))
                .GroupBy(g => g.CallId)
                .SelectMany(g => g.Where(c => c.State == CallEventStates.Accepted).Select(c => c.Date),
                    (c, cc) => c.Where(x => x.State != CallEventStates.Accepted)
                        .Select(r => new Call()
                        {
                            StartDate = cc,
                            Duration = r.Date - cc,
                            IsIncomming = r.State == CallEventStates.IncommingCallFinished,
                            OtherPortId = r.State == CallEventStates.IncommingCallFinished ? r.SourcePortId : r.DestinationPortId
                        }).FirstOrDefault())
                .Where(c => c.Duration != default(TimeSpan))
                .ToArray();                
        }

        public IEnumerable<Payment> Payments(IContract contract)
        {
            return _payments.Where(p => p.Contract.Equals(contract)).ToArray();
        }

        public bool Pay(IContract contract, double amount)
        {
            if (_subscribers.Any(s => s.Contract.Equals(contract)) && amount > 0)
            {
                _payments.Add(new Payment() {Amount = amount, Contract = contract, Date = DateTime.Now});
                return true;
            }

            return false;
        }

        public ISubscriber Subscribe(string subscriberName, ITariff tariff)
        {
            if (_subscribers.Any(s => s.Name.Equals(subscriberName)))
                return null;

            ITerminal terminal = new Terminal();
            IContract contract = _contractFactory.CreateContract(tariff, _phoneExchange.CreatePort());
            contract.ContractStateChanging += BillingContractStateChanging;
            ISubscriber subscriber = _subscriberFactory.CreateSubscriber(subscriberName, contract, terminal);
            _subscribers.Add(subscriber);

            return subscriber;
        }

        public bool Unsubscribe(ISubscriber subscriber)
        {
            if (_subscribers.Remove(subscriber))
            {
                subscriber.Contract.Port.Disconnect();
                _phoneExchange.RemovePort(subscriber.Contract.Port);
                _allowContractStateChange = true;
                subscriber.Contract.State = ContractStates.Completed;
                _allowContractStateChange = false;
                subscriber.Contract.ContractStateChanging -= BillingContractStateChanging;
            }

            return false;
        }

        public double Cost(IContract contract, Func<Call, bool> condition)
        {
            throw new NotImplementedException();
        }

        // Подписки

        public Billing(IBillableExchange phoneExchange, IContractFactory contractFactory, ISubscriberFactory subscriberFactory)
        {
            if (phoneExchange == null)
                throw new ArgumentNullException(nameof(phoneExchange), "phoneExchange cannot be null.");
            
            _phoneExchange = phoneExchange;
            _contractFactory = contractFactory;
            _subscriberFactory = subscriberFactory;

            _phoneExchange.CallLog += BillingCallLog;
            _phoneExchange.CallPermissionCheck += BillingCallPermissionCheck;
        }

        protected virtual void BillingCallLog(object sender, CallEventArgs e)
        {
            _callLog.Add(e);
        }

        protected virtual void BillingCallPermissionCheck(object sender, CallEventArgs e)
        {
            
        }
        
        // Подписка на изменение состояния контракта
        protected void BillingContractStateChanging(object sender, ContractStateChangeEventArgs e)
        {            
            e.ChangeAllowed = _allowContractStateChange;
        }
    }
}
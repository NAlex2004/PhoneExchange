using System;
using System.Collections.Generic;
using NAlex.APE.Event;
using NAlex.APE.Interfaces;
using NAlex.Billing.Interfaces;

namespace NAlex.Billing
{
    public class Billing: IBilling
    {
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
            throw new NotImplementedException();
        }

        public IEnumerable<Payment> Payments(IContract contract)
        {
            throw new NotImplementedException();
        }

        public void Pay(IContract contract, double amount)
        {
            throw new NotImplementedException();
        }

        public ISubscriber Subscribe(string subscriberName, ITariff tariff)
        {
            throw new NotImplementedException();
        }

        public bool Unsubscribe(ISubscriber subscriber)
        {
            throw new NotImplementedException();
        }

        public double Cost(IContract contract, Func<Call, bool> condition)
        {
            throw new NotImplementedException();
        }


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
    }
}
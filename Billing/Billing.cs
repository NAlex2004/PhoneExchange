using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using NAlex.APE;
using NAlex.APE.Enums;
using NAlex.APE.Event;
using NAlex.APE.Interfaces;
using NAlex.Billing.Events;
using NAlex.Billing.Interfaces;
using NAlex.Helpers;

namespace NAlex.Billing
{
	public class Billing: IBilling, IDisposable
    {
		private Timer _timer;
		private bool _disposed = false;

		private IDateTimeHelper _dtHelper;
        private bool _allowContractStateChange = false;
        
        private IBillableExchange _phoneExchange;
        private IContractFactory _contractFactory;
        private ISubscriberFactory _subscriberFactory;
        
        private IList<Call> _callLog = new List<Call>();
        // Значение словаря - начисления абонентской платы
        private IDictionary<ISubscriber, double> _subscribersFee = new Dictionary<ISubscriber, double>();
        private IList<Payment> _payments = new List<Payment>();

        public IEnumerable<ISubscriber> Subscribers
        {
            get { return _subscribersFee.Keys; }
        }
        
        public IEnumerable<Call> Calls(IContract contract)
        {
            return _callLog
                .Where(c => (c.SourcePortId.Equals(contract.Port.PortId) || c.DestinationPortId.Equals(contract.Port.PortId)) 
                            && c.Duration != TimeSpan.Zero);                                
        }

        public IEnumerable<Payment> Payments(IContract contract)
        {
            return _payments.Where(p => p.Contract.Equals(contract)).ToArray();
        }     
        
        public bool Pay(IContract contract, double amount)
        {
            if (_subscribersFee.Keys.Any(s => s.Contract.Equals(contract)) && amount > 0)
            {
                _payments.Add(new Payment() {Amount = amount, Contract = contract, Date = _dtHelper.Now});
                CheckContractBalance(contract);
                return true;
            }

            return false;
        }

        public ISubscriber Subscribe(string subscriberName, ITariff tariff)
        {
            if (_subscribersFee.Keys.Any(s => s.Name.Equals(subscriberName)))
                return null;

			ITerminal terminal = new Terminal(_dtHelper);
            IContract contract = _contractFactory.CreateContract(tariff, _phoneExchange.CreatePort());
            contract.ContractStateChanging += BillingContractStateChanging;
            ISubscriber subscriber = _subscriberFactory.CreateSubscriber(subscriberName, contract, terminal);
            _subscribersFee.Add(new KeyValuePair<ISubscriber, double>(subscriber, 0));

            return subscriber;
        }

        public bool Unsubscribe(ISubscriber subscriber)
        {
            if (Balance(subscriber.Contract, _dtHelper.Now) < 0)
                return false;
            
            if (_subscribersFee.Remove(subscriber))
            {
                subscriber.Contract.Port.Disconnect();
                _phoneExchange.RemovePort(subscriber.Contract.Port);
                _allowContractStateChange = true;
                subscriber.Contract.State = ContractStates.Completed;
                _allowContractStateChange = false;
                subscriber.Contract.ContractStateChanging -= BillingContractStateChanging;
                
                return true;
            }

            return false;
        }

        public virtual double Cost(IContract contract, Func<Call, bool> condition)
        {
            return Calls(contract).Where(condition)
                .Sum(c => c.SourcePortId.Equals(contract.Port.PortId)
                    ? c.SourceTariff.CallCost(c.SourcePortId, c)
                    : c.DestinationTariff.CallCost(c.DestinationPortId, c));
        }

        public virtual double Balance(IContract contract, DateTime date)
        {
            if (contract == null)
                return 0;
            if (contract.TariffStartDate > date)
                return 0;
            ISubscriber subscriber = _subscribersFee.Keys.FirstOrDefault(s => s.Contract.Equals(contract));
            if (subscriber == null)
                return 0;
            double sum = - Cost(contract, c => true)
                - _subscribersFee[subscriber] // начисленная абонентская
                + _payments.Where(p => p.Contract.Equals(contract)).Sum(p => p.Amount);

            return sum;
        }

        protected virtual void CheckContractBalance(IContract contract)
        {
            if (contract.State == ContractStates.Completed)
                return;
            
            double balance = Balance(contract, _dtHelper.Now);
            _allowContractStateChange = true;
            if (balance >= 0)
            {
                if (contract.State == ContractStates.Locked)
                    contract.State = ContractStates.Active;
            }
            else
            {
                if (contract.State == ContractStates.Active)
                    contract.State = ContractStates.Locked;
            }
            _allowContractStateChange = false;
        }

        protected virtual void CheckContracts()
        {
            foreach (IContract contract in _subscribersFee.Keys.Where(s => s.Contract.State != ContractStates.Completed).Select(s => s.Contract))
            {
                if (contract.PaymentDay >= _dtHelper.Now.Day)
                    CheckContractBalance(contract);
            }
        }

        protected virtual void AddFee(int days)
        {
            foreach (var subscriber in _subscribersFee.Keys)
            {
                _subscribersFee[subscriber] += subscriber.Contract.Tariff.TotalFee(days);
            }
        }
        
        // Подписка на событие, происходящее, скажем, раз в день для проверок и расчетов
        protected virtual void DailyTask(object sender, EventArgs e)
        {
            AddFee(1);
            CheckContracts();
        }
        
        // Подписки

		public Billing(IBillableExchange phoneExchange, IContractFactory contractFactory, ISubscriberFactory subscriberFactory, IDateTimeHelper dtHelper = null)
        {
            if (phoneExchange == null)
                throw new ArgumentNullException(nameof(phoneExchange), "phoneExchange cannot be null.");

			_dtHelper = dtHelper ?? new DefaultDateTimeHelper();

            _phoneExchange = phoneExchange;
            _contractFactory = contractFactory;
            _subscriberFactory = subscriberFactory;

            _phoneExchange.CallLog += BillingCallLog;
            _phoneExchange.CallPermissionCheck += BillingCallPermissionCheck;

			_timer = new Timer(_dtHelper.DayInterval);
			_timer.AutoReset = true;
			_timer.Elapsed += DailyTask;
			_timer.Start();
        }

        protected virtual void BillingCallLog(object sender, CallEventArgs e)
        {
            Call call;
            
            switch (e.State)
            {
                case CallEventStates.Accepted:
                    call  = _callLog.FirstOrDefault(c => c.CallId == e.CallId);
                    if (call == null)
                    {
                        call = new Call()
                        {
                            CallId = e.CallId,
                            SourcePortId = e.SourcePortId,
                            DestinationPortId = e.DestinationPortId,
                            StartDate = e.Date,
                            SourceTariff = _subscribersFee.Keys.Where(s => s.Contract.Port.PortId.Equals(e.SourcePortId))
                                .Select(s => s.Contract.Tariff)
                                .FirstOrDefault(),
                            DestinationTariff = _subscribersFee.Keys.Where(s => s.Contract.Port.PortId.Equals(e.DestinationPortId))
                                .Select(s => s.Contract.Tariff)
                                .FirstOrDefault(),
                            Duration = TimeSpan.Zero
                        };                        
                        
                        _callLog.Add(call);
                    }       
                    break;
                case CallEventStates.IncommingCallFinished:
                case CallEventStates.OutgoingCallFinished:
                    call  = _callLog.FirstOrDefault(c => c.CallId == e.CallId);
                    if (call != null)
                    {
                        if (call.Duration == TimeSpan.Zero)
                        {                                                        
                            call.Duration = e.Date - call.StartDate;
                        }
                    }
                    break;             
            }
        }

        protected virtual void BillingCallPermissionCheck(object sender, CallEventArgs e)
        {
            ISubscriber subscriber = _subscribersFee.Keys.FirstOrDefault(s => s.Contract.Port.PortId.Equals(e.SourcePortId));
            if (subscriber != null && subscriber.Contract.State == ContractStates.Active)
                e.IsAllowed = true;
            else
                e.IsAllowed = false;
        }
        
        // Подписка на изменение состояния контракта
        protected void BillingContractStateChanging(object sender, ContractStateChangeEventArgs e)
        {            
            e.ChangeAllowed = _allowContractStateChange;
        }

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			_timer.Stop();
			_timer.Elapsed -= DailyTask;
			_timer.Dispose();

			_disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
		}
	}
}
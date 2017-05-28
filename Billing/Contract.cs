using System;
using System.Linq;
using NAlex.Billing.Interfaces;
using NAlex.APE.Interfaces;
using NAlex.Billing.Events;
using NAlex.Helpers;

namespace NAlex.Billing
{
	public class Contract : IContract
	{
		private readonly int _daysToChangeContract;
		private ContractStates _state;
		private IDateTimeHelper _dtHelper;
		
		public DateTime Date { get; protected set; }
		public ITariff Tariff { get; protected set; }
		public DateTime TariffStartDate { get; protected set; }
		public IPort Port { get; protected set; }
		public int PaymentDay { get; protected set; }
		public event ContractStateChangeEventHandler ContractStateChanging;

		public ContractStates State
		{
			get { return _state; }
			set
			{
				ContractStateChangeEventArgs e = new ContractStateChangeEventArgs()
				{
					Contract = this,
					OldState = _state,
					NewState = value,
					ChangeAllowed = false
				};

				OnContractStateChanging(e);

				if (e.ChangeAllowed)
					_state = value;
			}
		}

		protected virtual void OnContractStateChanging(ContractStateChangeEventArgs e)
		{
			if (ContractStateChanging == null) 
				return;
			
			var invList = ContractStateChanging.GetInvocationList();
			var billingHandlers = invList
				.Where(d => d.Target is IBilling)
				.OfType<ContractStateChangeEventHandler>();

			var otherHandlers = invList.Where(d => !(d.Target is IBilling)).OfType<ContractStateChangeEventHandler>();
			
			bool allow = e.ChangeAllowed;
		
			foreach (var handler in otherHandlers)
			{
				handler(this, e);
			}

			e.ChangeAllowed = allow;
			
			foreach (var handler in billingHandlers)
			{
				handler(this, e);
			}
		}
		
		public virtual bool ChangeTariff(IBilling billing, ITariff newTariff)
		{
			bool res = newTariff != null && (_dtHelper.Now - TariffStartDate).Days >= _daysToChangeContract && billing.Balance(this, _dtHelper.Now) >= 0;
			if (res)
			{				
				Tariff = newTariff;
				TariffStartDate = _dtHelper.Now;
			}
			return res;
		}

		public Contract(ITariff tariff, IPort port, int paymentDay, int daysToChangeContract, IDateTimeHelper dtHelper = null)
		{
			if (tariff == null)
				throw new ArgumentNullException(nameof(tariff), "tariff cannot be null.");
			if (port == null)
				throw new ArgumentNullException(nameof(port), "port cannot be null.");

			_dtHelper = dtHelper ?? new DefaultDateTimeHelper();

			Tariff = tariff;
			Port = port;
			PaymentDay = paymentDay;
			Date = _dtHelper.Now;
			TariffStartDate = Date;
			State = ContractStates.Active;
			_daysToChangeContract = daysToChangeContract;
		}
	}
}

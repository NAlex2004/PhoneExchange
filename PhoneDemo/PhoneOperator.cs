using System;
using NAlex.APE;
using NAlex.APE.Event;
using NAlex.APE.Factories;
using NAlex.APE.Interfaces;
using NAlex.APE.Enums;
using NAlex.Billing;
using NAlex.Billing.Factories;
using NAlex.Billing.Interfaces;
using NAlex.Helpers;
using NAlex.Billing.Tariffs;

namespace PhoneDemo
{
	public class PhoneOperator: IDisposable
	{
		IDateTimeHelper dtHelper;
		IBillableExchange ape;
		public IBilling Billing { get; protected set; }

		public PhoneOperator()
		{
			dtHelper = new DateTimeHelper(100000);

			IPortFactory pFactory = new PePortFactory();
			ape = new PhoneExchange(pFactory, (new IntId()).StartValue());
			Billing = new Billing(ape, new ContractFactory(dtHelper), new SubscriberFactory(), dtHelper);
		}

		public void CreateSomeSubscribers()
		{
			
		}

		public void MakeSomeCalls()
		{
			
		}

		public void WriteSomeStatistics()
		{
			
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					Billing.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);		
		}
		#endregion
	}
}

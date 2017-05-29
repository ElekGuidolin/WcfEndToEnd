using GeoLib.Contracts;
using GeoLib.Data;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Transactions;

namespace GeoLib.Services
{
    //This is the only one behavior that can be set here.
    //[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    //Always set this inline, and not in config to don't risk yourself on coding for an InstanceContextMode, and the config file with another one.
    //This, obviously, because the config doesn't need a build to be changed.

    //InstanceContextMode.PerCall and ConcurrencyMode = ConcurrencyMode.Single will reload _Counter at every call.
    //InstanceContextMode.PerSession and ConcurrencyMode = ConcurrencyMode.Single will increment _Counter at every call.
    //InstanceContextMode.Single and ConcurrencyMode = ConcurrencyMode.Single will increment _Counter at every call, but it will response one per call even with multiple client call.

    //InstanceContextMode.PerCall and ConcurrencyMode = ConcurrencyMode.Multiple will start a lot of hosts, but reset _Counter.
    //InstanceContextMode.PerSession and ConcurrencyMode = ConcurrencyMode.Multiple will increment _Counter at every call, but it have no control under the last call. The last response shown, it is the lastest client's confirmation.
    //InstanceContextMode.Single and ConcurrencyMode = ConcurrencyMode.Multiple will increment the _Counter, but return different responses for each client call.

    //ReleaseServiceInstanceOnTransactionComplete = false inside ServiceBehavior is required when InstanceContextMode = PerSession
    [ServiceBehavior(ReleaseServiceInstanceOnTransactionComplete = false,
                    InstanceContextMode = InstanceContextMode.PerCall)]
	public class GeoManager : IGeoService
	{
		#region Fields

		IZipCodeRepository _zipCodeRepository = null;
		IStateRepository _stateRepository = null;
		int _Counter = 0;

		#endregion

		#region Constructors

		public GeoManager()
		{
		}

		public GeoManager(IZipCodeRepository pZipCodeRepository) : this(pZipCodeRepository, null)
		{
		}

		public GeoManager(IStateRepository pStateRepository) : this(null, pStateRepository)
		{
		}

		public GeoManager(IZipCodeRepository pZipCodeRepository, IStateRepository pStateRepository)
		{
			_zipCodeRepository = pZipCodeRepository;
			_stateRepository = pStateRepository;
		}

		#endregion

		#region IGeoService Implementation

		public ZipCodeData GetZipInfo(string zip)
		{
			//Test after set timeout to 5 seconds. It throws a timeout exception.
			//Thread.Sleep(10000);

			ZipCodeData zipCodeData = null;

			IZipCodeRepository zipCodeRepository = _zipCodeRepository ?? new ZipCodeRepository();

			ZipCode zipCodeEntity = zipCodeRepository.GetByZip(zip);
			if (zipCodeEntity != null)
			{
				zipCodeData = new ZipCodeData()
				{
					City = zipCodeEntity.City,
					State = zipCodeEntity.State.Abbreviation,
					ZipCode = zipCodeEntity.Zip
				};
			}
            else
            {
				//Used as Unhandled Exception. Sets the proxy state at the client to "Faulted"
				//throw new ApplicationException(string.Format("Zip code {0} not found.", zip));

				//Used as Handled Exception. Keeps the proxy state at the client to "Opened"
				//throw new FaultException(string.Format("Zip code {0} not found.", zip));

				//Used with FaultContract that must be declared at the contract level, the IGeoService in this case. Keeps the proxy state at the client to "Opened"
				//ApplicationException ex = new ApplicationException(string.Format("Zip code {0} not found.", zip));
				//throw new FaultException<ApplicationException>(ex, "Just another message");

				//Better and most powerful way to throw an exception. Using FaultContract that must be declared at the contract level, 
				//and using a custom Class to define and send the information needed. Keeps the proxy state at the client to "Opened"
				NotFoundData data = new NotFoundData()
				{
					Message = string.Format("Zip code {0} not found.", zip),
					When = DateTime.Now.ToString(),
					User = "elguidolin" //It can be recovered by Identity or anythin else.
				};

				throw new FaultException<NotFoundData>(data, "Just another message.");
			}

			//lock (this)
			//{
			//    _Counter++;
			//}

			//MyStaticResource.DoSomething();

			//Keep this only if the Host is a Console.
			//Console.WriteLine("Count = {0}", _Counter.ToString());

			//Used in Instancing and Concurrency for demonstration. Need reference to PresentationFramework and using System.Window;
			//MessageBox.Show(string.Format("{0} = {1}, {2}", zip, zipCodeData.City, zipCodeData.State), "Call Counter " + _Counter.ToString());

			return zipCodeData;
		}

		public IEnumerable<string> GetState(bool primaryOnly)
		{
			List<string> stateData = new List<string>();

			IStateRepository stateRepository = _stateRepository ?? new StateRepository();

			IEnumerable<State> states = stateRepository.Get(primaryOnly);

			if (states != null)
			{
				foreach (State state in states)
				{
					stateData.Add(state.Abbreviation);
				}
			}

			return stateData;
		}

		public IEnumerable<ZipCodeData> GetZips(string state)
		{
			List<ZipCodeData> zipCodeData = new List<ZipCodeData>();

			IZipCodeRepository zipCodeRepository = _zipCodeRepository ?? new ZipCodeRepository();

			var zips = zipCodeRepository.GetByState(state);
			if (zips != null)
			{
				foreach (ZipCode zipCode in zips)
				{
					zipCodeData.Add(new ZipCodeData()
					{
						City = zipCode.City,
						State = zipCode.State.Abbreviation,
						ZipCode = zipCode.Zip
					});
				}
			}

			return zipCodeData;
		}

		public IEnumerable<ZipCodeData> GetZips(string zip, int range)
		{
			List<ZipCodeData> zipCodeData = new List<ZipCodeData>();

			IZipCodeRepository zipCodeRepository = _zipCodeRepository ?? new ZipCodeRepository();

			ZipCode zipEntity = zipCodeRepository.GetByZip(zip);
			IEnumerable<ZipCode> zips = zipCodeRepository.GetZipsForRange(zipEntity, range);
			if (zips != null)
			{
				foreach (ZipCode zipCode in zips)
				{
					zipCodeData.Add(new ZipCodeData()
					{
						City = zipCode.City,
						State = zipCode.State.Abbreviation,
						ZipCode = zipCode.Zip
					});
				}
			}

			return zipCodeData;
		}

		[OperationBehavior(TransactionScopeRequired = true)]
		public void UpdateZipCity(string zip, string city)
		{
			IZipCodeRepository zipCodeRepository = _zipCodeRepository ?? new ZipCodeRepository();

			ZipCode zipEntity = zipCodeRepository.GetByZip(zip);
			if (zipEntity != null)
			{
				zipEntity.City = city;
				zipCodeRepository.Update(zipEntity);
			}
		}

        //Setting the TransactionScopeRequired = true will make this method a transaction friendly method, wich means that if a transaction already exists, and call this method,
        //it will be part of the transaction.If no transaction exists, it will start a new one, and everything inside the method, will be controlled by this transaction.
        //Setting to false, even if a transaction already exists, this method will not be controlled by the transaction.
        //To perform manual transaction commit, set TransactionAutoComplete = false
        [OperationBehavior(TransactionScopeRequired = true)]
        //[OperationBehavior(TransactionScopeRequired = false)]
        public void UpdateZipCity(IEnumerable<ZipCityData> zipCityData)
        {
            IZipCodeRepository zipCodeRepository = _zipCodeRepository ?? new ZipCodeRepository();

            #region Good code. Try to do this way always.

            //Dictionary<string, string> cityBatch = new Dictionary<string, string>();

            //foreach (ZipCityData zipCityItem in zipCityData)
            //{
            //	cityBatch.Add(zipCityItem.Zip, zipCityItem.City);
            //}

            //zipCodeRepository.UpdateCityBatch(cityBatch);

            #endregion

            #region Throw Error

            int counter = 0;

            foreach (ZipCityData zipCityItem in zipCityData)
            {
                counter++;

				//This comment is to test throwing the error at the client.
                //if (counter == 2)
                //{
                //    //If TransactionAutoComplete = false, the below throw can not be raised, instead, do not "void",
                //    //return something through the method.
                //    throw new FaultException("Sorry, can't touch this!");
                //}

                ZipCode zipCodeEntity = zipCodeRepository.GetByZip(zipCityItem.Zip);
                zipCodeEntity.City = zipCityItem.City;
                ZipCode updateItem = zipCodeRepository.Update(zipCodeEntity);
            }

            #endregion

            #region Manual Transaction Programming

            //Default is TransactionScopeOption.Required
            //using (TransactionScope scope = new TransactionScope())
            //{
            //    int counter = 0;

            //    foreach (ZipCityData zipCityItem in zipCityData)
            //    {
            //        counter++;

            //        if (counter == 2)
            //        {
            //            throw new FaultException("Sorry, can't touch this!");
            //        }

            //        ZipCode zipCodeEntity = zipCodeRepository.GetByZip(zipCityItem.Zip);
            //        zipCodeEntity.City = zipCityItem.City;
            //        ZipCode updateItem = zipCodeRepository.Update(zipCodeEntity);
            //    }

            //    scope.Complete();
            //}

            #endregion

            //To perform manual transaction commit, set TransactionAutoComplete = false, and proceed the line below:
            //OperationContext.Current.SetTransactionComplete();
        }

        #endregion
    }
}

public static class MyStaticResource
{
    static object _Lock = new object();

    public static void DoSomething()
    {
        lock (_Lock)
        {
            //Ensuring only one run at a time
        }
    }
}

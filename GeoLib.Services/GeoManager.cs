using GeoLib.Contracts;
using GeoLib.Data;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

namespace GeoLib.Services
{
	//This is the only one behavior that can be set here.
	//[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	//Always set this inline, and not in config to don't risk yourself on coding for an InstanceContextMode, and the config file with another one.
	//This, obviously, because the config doesn't need a build to be changed.
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
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

			_Counter++;
			//Kepp this only if the Host is a Console.
			Console.WriteLine("Count = {0}", _Counter.ToString());

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

		#endregion
	}
}

using GeoLib.Contracts;
using GeoLib.Data;
using System.Collections.Generic;
using System.ServiceModel;

namespace GeoLib.Services
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
	public class StatefulGeoManager : IStatefulGeoService
	{
		ZipCode _ZipCodeEntity;

		public void PushZip(string zip)
		{
			IZipCodeRepository zipCodeRepository = new ZipCodeRepository();
			_ZipCodeEntity = zipCodeRepository.GetByZip(zip);
		}

		public ZipCodeData GetZipInfo()
		{
			ZipCodeData zipCodeData = null;

			if (_ZipCodeEntity != null)
			{
				zipCodeData = new ZipCodeData()
				{
					City = _ZipCodeEntity.City,
					State = _ZipCodeEntity.State.Abbreviation,
					ZipCode = _ZipCodeEntity.Zip
				};
			}

			return zipCodeData;
		}

		public IEnumerable<ZipCodeData> GetZips(int range)
		{
			List<ZipCodeData> zipCodeData = new List<ZipCodeData>();

			if (_ZipCodeEntity != null)
			{
				IZipCodeRepository zipCodeRepository = new ZipCodeRepository();
				IEnumerable<ZipCode> zips = zipCodeRepository.GetZipsForRange(_ZipCodeEntity, range);
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
			}

			return zipCodeData;
		}

	}
}

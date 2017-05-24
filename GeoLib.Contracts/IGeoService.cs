using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace GeoLib.Contracts
{
	[ServiceContract]
	public interface IGeoService
	{
		[OperationContract]
		[FaultContract(typeof(ApplicationException))]
		[FaultContract(typeof(NotFoundData))]
		ZipCodeData GetZipInfo(string zip);

		[OperationContract]
		IEnumerable<string> GetState(bool primaryOnly);

		[OperationContract(Name = "GetZipsByState")]
		IEnumerable<ZipCodeData> GetZips(string state);

		[OperationContract(Name ="GetZipsForRange")]
		IEnumerable<ZipCodeData> GetZips(string zip, int range);

		[OperationContract]
		void UpdateZipCity(string zip, string city);

		[OperationContract]
		void UpdateZipCity(IEnumerable<ZipCityData> zipCityData);
	}
}

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

		[OperationContract(Name = "UpdateCityByZip")]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdateZipCity(string zip, string city);

		//The code below blocks the propagation of transactions from client to server.
		//When it's NotAllowed, will mean that if the service part doesn't throw any errors, the client will not rollback if catches an error.
		//[TransactionFlow(TransactionFlowOption.NotAllowed)]
		[OperationContract(Name = "UpdateCityRange")]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdateZipCity(IEnumerable<ZipCityData> zipCityData);
	}
}

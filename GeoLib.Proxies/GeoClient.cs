using GeoLib.Contracts;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System;

namespace GeoLib.Proxies
{
	public class GeoClient : ClientBase<IGeoService>, IGeoService
	{
		public GeoClient(string endpointName)
			: base(endpointName)
		{}

		public GeoClient(Binding binding, EndpointAddress address)
			: base(binding, address)
		{}

		public IEnumerable<string> GetState(bool primaryOnly)
		{
			return Channel.GetState(primaryOnly);
		}

		public ZipCodeData GetZipInfo(string zip)
		{
			return Channel.GetZipInfo(zip);
		}

		public IEnumerable<ZipCodeData> GetZips(string state)
		{
			return Channel.GetZips(state);
		}

		public IEnumerable<ZipCodeData> GetZips(string zip, int range)
		{
			return Channel.GetZips(zip, range);
		}

		public void UpdateZipCity(string zip, string city)
		{
			Channel.UpdateZipCity(zip, city);
		}

		public void UpdateZipCity(IEnumerable<ZipCityData> zipCityData)
		{
			Channel.UpdateZipCity(zipCityData);
		}
	}
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel.Channels;
using System.ServiceModel;
using GeoLib.Services;
using GeoLib.Contracts;

namespace GeoLib.Tests
{
	[TestClass]
	public class ServiceTests
	{
		[TestMethod]
		public void Test_zip_code_retrieval()
		{
			//Arrange
			string address = "net.pipe://localhost/GeoService";
			Binding binding = new NetNamedPipeBinding();

			ServiceHost host = new ServiceHost(typeof(GeoManager));
			host.AddServiceEndpoint(typeof(IGeoService), binding, address);
			host.Open();

			ChannelFactory<IGeoService> factory = new ChannelFactory<IGeoService>(binding, new EndpointAddress(address));
			IGeoService proxy = factory.CreateChannel();

			//Act
			ZipCodeData data = proxy.GetZipInfo("07035");

			//Assert
			Assert.AreEqual(data.City.ToUpper(), "LINCOLN PARK");
			Assert.AreEqual(data.State, "NJ");
		}
	}
}

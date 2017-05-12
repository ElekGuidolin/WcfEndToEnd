using GeoLib.Contracts;
using GeoLib.Services;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace GeoLib.ConsoleHost
{
	class Program
	{
		static void Main(string[] args)
		{
			ServiceHost hostGeoManager = new ServiceHost(typeof(GeoManager));
			hostGeoManager.Open();

			ServiceHost hostStatefulGeoManager = new ServiceHost(typeof(StatefulGeoManager));
			hostStatefulGeoManager.Open();

			#region Programmatically Configuration (Commented)

			//Configuring programatically and comment to the App.config. Uncomment there, comment here for no errors.
			//string address = "net.tcp://localhost:8009/GeoService";
			//Binding binding = new NetTcpBinding();
			//Type contract = typeof(IGeoService);

			//hostGeoManager.AddServiceEndpoint(contract, binding, address);

			#endregion

			#region Programmatically Mex and baseAddress configuration. (Also Commented)

			//Same configuration that is actually in the App.Config, but programmatically.

			//ServiceHost hostMexGeoManager = new ServiceHost(typeof(GeoManager), new Uri("http://localhost:1234"), new Uri("net.tcp://localhost:8009"));
			//ServiceMetadataBehavior behavior = hostMexGeoManager.Description.Behaviors.Find<ServiceMetadataBehavior>();
			//if (behavior == null)
			//{
			//	behavior = new ServiceMetadataBehavior()
			//	{
			//		HttpGetEnabled = true
			//	};
			//	hostMexGeoManager.Description.Behaviors.Add(behavior);
			//}

			//hostMexGeoManager.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "MEX");

			#endregion

			Console.WriteLine("Services started. Press [Enter] to exit");
			Console.ReadKey();

			hostGeoManager.Close();
			hostStatefulGeoManager.Close();
		}
	}
}

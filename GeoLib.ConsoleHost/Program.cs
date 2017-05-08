using GeoLib.Contracts;
using GeoLib.Services;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace GeoLib.ConsoleHost
{
	class Program
	{
		static void Main(string[] args)
		{
			ServiceHost hostGeoManager = new ServiceHost(typeof(GeoManager));

			#region Programatically Configuration (Commented)

			//Configuring programatically and comment to the App.config. Uncomment there, comment here for no errors.
			//string address = "net.tcp://localhost:8009/GeoService";
			//Binding binding = new NetTcpBinding();
			//Type contract = typeof(IGeoService);

			//hostGeoManager.AddServiceEndpoint(contract, binding, address);

			#endregion

			hostGeoManager.Open();

			Console.WriteLine("Services started. Press [Enter] to exit");
			Console.ReadKey();

			hostGeoManager.Close();
		}
	}
}

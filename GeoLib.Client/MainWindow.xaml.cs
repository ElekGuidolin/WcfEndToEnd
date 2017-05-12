using GeoLib.Client.Contracts;
using GeoLib.Contracts;
using GeoLib.Proxies;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;

namespace GeoLib.Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		StatefulGeoClient _Proxy;

		public MainWindow()
		{
			InitializeComponent();
			_Proxy = new StatefulGeoClient();
		}

		private void btnGetInfo_Click(object sender, RoutedEventArgs e)
		{
			if (!cbxStateful.IsChecked.Value)
			{
				GetZipInfoCommon();
			}
			else
			{
				GetZipInfoStateful();
			}
		}

		private void GetZipInfoCommon()
		{
			if (!string.IsNullOrWhiteSpace(txtZipSearch.Text))
			{
				//This string sent to GeoClient constructor will define which configuration will be used. Very flexible. Check App.Config.
				//This call, in the way it's configured, will only work with WindowsHost. For WebHost, use webEP.
				GeoClient proxy = new GeoClient("tcpEP");

				ZipCodeData data = proxy.GetZipInfo(txtZipSearch.Text);
				if (data != null)
				{
					lblResponseCity.Content = data.City;
					lblResponseState.Content = data.State;
				}

				proxy.Close();
			}
		}

		private void GetZipInfoStateful()
		{
			if (!string.IsNullOrWhiteSpace(txtZipSearch.Text))
			{
				ZipCodeData data = _Proxy.GetZipInfo();
				if (data != null)
				{
					lblResponseCity.Content = data.City;
					lblResponseState.Content = data.State;
				}
			}
		}


		private void btnGetZipCodes_Click(object sender, RoutedEventArgs e)
		{
			//This call, in the way it's configured, will only work with ConsoleHost
			if (!string.IsNullOrWhiteSpace(txtSearchState.Text))
			{
				//As commented in the module, it's possible to set values in the config file or programmatically.
				//Here it was set programmatically, to change this, you just need to comment the next lines,
				//and make the constructor passing the tcpEP string as parameter.
				EndpointAddress address = new EndpointAddress("net.tcp://localhost:8009/GeoService");
				Binding binding = new NetTcpBinding()
				{
					MaxReceivedMessageSize = 2000000,
					SendTimeout = new TimeSpan(0, 0, 5)
				};

				GeoClient proxy = new GeoClient(binding, address);

				IEnumerable<ZipCodeData> data = proxy.GetZips(txtSearchState.Text);
				if (data != null)
				{
					lstZips.ItemsSource = data;
				}

				proxy.Close();
			}
		}

		private void btnMakeCall_Click(object sender, RoutedEventArgs e)
		{
			//Not fixed bug. Need to call with "" parameter. Same as three line below, but without .config file.
			//To uncomment, need to uncomment .config, and comment below.
			//ChannelFactory<IMessageService> factory = new ChannelFactory<IMessageService>("");

			//This call, in the way it's configured, will only work with WindowsHost
			EndpointAddress address = new EndpointAddress("net.tcp://localhost:8010/MessageService");
			Binding binding = new NetTcpBinding();
			ChannelFactory<IMessageService> factory = new ChannelFactory<IMessageService>(binding, address);

			IMessageService proxy = factory.CreateChannel();

			proxy.ShowMsg(txtTextToShow.Text);

			factory.Close();
		}

		private void btnPush_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(txtZipSearch.Text))
			{
				_Proxy.PushZip(txtZipSearch.Text);
			}
		}

		private void btnGetInRange_Click(object sender, RoutedEventArgs e)
		{
			if ((!string.IsNullOrWhiteSpace(txtZipSearch.Text)) && (!string.IsNullOrWhiteSpace(txtRange.Text)))
			{
				IEnumerable<ZipCodeData> data = _Proxy.GetZips(Convert.ToInt32(txtRange.Text));
				if (data != null)
				{
					lstZips.ItemsSource = data;
				}
			}
		}
	}
}

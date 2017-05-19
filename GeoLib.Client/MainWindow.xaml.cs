using GeoLib.Client.Contracts;
using GeoLib.Contracts;
using GeoLib.Proxies;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Windows;

namespace GeoLib.Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		GeoClient _Proxy;
		StatefulGeoClient _ProxyStateful;
		SynchronizationContext _SyncContext = null;

		public MainWindow()
		{
			InitializeComponent();

			//This string sent to GeoClient constructor will define which configuration will be used. Very flexible. Check App.Config.
			//This call, in the way it's configured, will only work with WindowsHost. For WebHost, use webEP.
			_Proxy = new GeoClient("tcpEP");
			_ProxyStateful = new StatefulGeoClient();
			_SyncContext = SynchronizationContext.Current;
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
			string zipToSearch = txtZipSearch.Text;
			if (!string.IsNullOrWhiteSpace(zipToSearch))
			{
				Thread thread = new Thread(() =>
				{
					ZipCodeData data = _Proxy.GetZipInfo(zipToSearch);
					if (data != null)
					{
						SendOrPostCallback callback = new SendOrPostCallback(arg =>
						{
							lblResponseCity.Content = data.City;
							lblResponseState.Content = data.State;
						});

						_SyncContext.Send(callback, null);
					}
				})
				{
					IsBackground = true
				};

				thread.Start();
			}
		}

		private void GetZipInfoStateful()
		{
			if (!string.IsNullOrWhiteSpace(txtZipSearch.Text))
			{
				ZipCodeData data = _ProxyStateful.GetZipInfo();
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
			//Not fixed bug. Need to call new ChannelFactory with "" parameter. Same as three line below, but without .config file.
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
				_ProxyStateful.PushZip(txtZipSearch.Text);
			}
		}

		private void btnGetInRange_Click(object sender, RoutedEventArgs e)
		{
			if ((!string.IsNullOrWhiteSpace(txtZipSearch.Text)) && (!string.IsNullOrWhiteSpace(txtRange.Text)))
			{
				IEnumerable<ZipCodeData> data = _ProxyStateful.GetZips(Convert.ToInt32(txtRange.Text));
				if (data != null)
				{
					lstZips.ItemsSource = data;
				}
			}
		}

	}
}

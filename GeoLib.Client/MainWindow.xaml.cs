using GeoLib.Client.Contracts;
using GeoLib.Contracts;
using GeoLib.Proxies;
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
		public MainWindow()
		{
			InitializeComponent();
		}

		private void btnGetInfo_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(txtZipSearch.Text))
			{
                //This string define wich configuration will be used. Very flexible.
                //Keep tcpEP, but last test was webEP.
                GeoClient proxy = new GeoClient("webEP");

				ZipCodeData data = proxy.GetZipInfo(txtZipSearch.Text);
				if (data != null)
				{
					lblResponseCity.Content = data.City;
					lblResponseState.Content = data.State;
				}

				proxy.Close();
			}
		}

		private void btnGetZipCodes_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(txtSearchState.Text))
			{
				EndpointAddress address = new EndpointAddress("net.tcp://localhost:8009/GeoService");
				Binding binding = new NetTcpBinding();

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

            EndpointAddress address = new EndpointAddress("net.tcp://localhost:8010/MessageService");
            Binding binding = new NetTcpBinding();
            ChannelFactory<IMessageService> factory = new ChannelFactory<IMessageService>(binding, address);

            IMessageService proxy = factory.CreateChannel();

			proxy.ShowMsg(txtTextToShow.Text);

			factory.Close();
		}
	}
}

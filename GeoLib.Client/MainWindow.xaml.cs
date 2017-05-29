using GeoLib.Client.Contracts;
using GeoLib.Contracts;
using GeoLib.Proxies;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
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

            //Sleeping to have the chance to click Start Service on WindowsHost.
            Thread.Sleep(3000);
            _Proxy.Open();

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

        //Old way to do with thread. From now on use the async way.
        //private void GetZipInfoCommon()
        //{
        //	string zipToSearch = txtZipSearch.Text;
        //	if (!string.IsNullOrWhiteSpace(zipToSearch))
        //	{
        //		Thread thread = new Thread(() =>
        //		{
        //			ZipCodeData data = _Proxy.GetZipInfo(zipToSearch);
        //			if (data != null)
        //			{
        //				SendOrPostCallback callback = new SendOrPostCallback(arg =>
        //				{
        //					lblResponseCity.Content = data.City;
        //					lblResponseState.Content = data.State;
        //				});

        //				_SyncContext.Send(callback, null);
        //			}
        //		})
        //		{
        //			IsBackground = true
        //		};

        //		thread.Start();
        //	}
        //}

        private async void GetZipInfoCommon()
        {
			try
			{
				string zipToSearch = txtZipSearch.Text;
				if (!string.IsNullOrWhiteSpace(zipToSearch))
				{
					await Task.Run(() =>
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
					});
				}
			}
			catch (FaultException<ExceptionDetail> ex)
			{
				MessageBox.Show("FaultException<ExceptionDetail> thrown by service.\n\rException Type: " +
					"FaultException<ExceptionDetail>\n\r" +
					"Message: " + ex.Detail.Message + "\n\r" +
					"Proxy State: " + _Proxy.State.ToString());
			}
			catch (FaultException<ApplicationException> ex)
			{
				MessageBox.Show("FaultException<ApplicationException> thrown by service.\n\rException Type: " +
					"FaultException<ApplicationException>\n\r" +
					"Reason: " + ex.Message + "\n\r" +
					"Message: " + ex.Detail.Message + "\n\r" +
					"Proxy State: " + _Proxy.State.ToString());
			}
			catch(FaultException<NotFoundData> ex)
			{
				MessageBox.Show("FaultException<NotFoundData> thrown by service.\n\rException Type: " +
					"FaultException<NotFoundData>\n\r" +
					"Reason: " + ex.Message + "\n\r" +
					"Message: " + ex.Detail.Message + "\n\r" +
					"Proxy State: " + _Proxy.State.ToString());
			}
			catch (FaultException ex)
			{
				MessageBox.Show("FaultException thrown by service.\n\rException Type: " +
					ex.GetType().Name + "\n\r" +
					"Message: " + ex.Message + "\n\r" +
					"Proxy State: " + _Proxy.State.ToString());
			}
			catch (Exception ex)
			{
				MessageBox.Show("Exception thrown by service.\n\rException Type: " +
					ex.GetType().Name + "\n\r" +
					"Message: " + ex.Message + "\n\r" +
					"Proxy State: " + _Proxy.State.ToString());
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

		private void btnUpdateBatch_Click(object sender, RoutedEventArgs e)
		{
			List<ZipCityData> cityZipList = new List<ZipCityData>()
			{
				new ZipCityData() { Zip = "07035", City = "Bedrock"},
				new ZipCityData() { Zip = "33030", City = "End of the World"}
			};

			try
			{
				#region Controlling Transaction in the service

				//Way to do with transaction controlled by the service.
				//Must use TransactionScope in the service implementation.
				//GeoClient proxy = new GeoClient("tcpEP");
				//proxy.UpdateZipCity(cityZipList);
				//proxy.Close();

				#endregion

				#region Controlling Transactions PerCall at the client

				////Way to do with transaction controlled by the client.
				//GeoClient proxy = new GeoClient("tcpEP");

				////In the service, if the ReleaseServiceInstanceOnTransactionComplete = false is set,
				//using (TransactionScope scope = new TransactionScope())
				//            {
				//	//this method must have the [OperationBehavior(TransactionScopeRequired = true)] also set.
				//	proxy.UpdateZipCity(cityZipList);
				//                scope.Complete();
				//            }

				//            proxy.Close();

				#endregion

				#region Controlling Transaction PerSession failing at client, to make sure the propagation.

				//Way to do with transaction controlled by the client.
				GeoClient proxy = new GeoClient("tcpEP");

				//In the service, if the ReleaseServiceInstanceOnTransactionComplete = false is set,
				using (TransactionScope scope = new TransactionScope())
				{
					//this method must have the [OperationBehavior(TransactionScopeRequired = true)] also set.
					proxy.UpdateZipCity(cityZipList);
					//If the service is PerSession, it's necessary to close the proxy inside the transaction scope.
					proxy.Close();

					throw new ApplicationException("uh oh.");

					scope.Complete();
				}

				#endregion

				MessageBox.Show("Updated");
            }
            catch (Exception ex)
			{
				MessageBox.Show("Error");
			}
		}

		private void btnPutBack_Click(object sender, RoutedEventArgs e)
		{
			List<ZipCityData> cityZipList = new List<ZipCityData>()
			{
				new ZipCityData() { Zip = "07035", City = "Lincoln Park"},
				new ZipCityData() { Zip = "33030", City = "Homestead"}
			};

			try
			{
				GeoClient proxy = new GeoClient("tcpEP");
				proxy.UpdateZipCity(cityZipList);
				proxy.Close();

				MessageBox.Show("Updated");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error");
			}
		}
	}
}

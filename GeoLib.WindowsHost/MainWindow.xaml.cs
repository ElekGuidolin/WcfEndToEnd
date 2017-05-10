using GeoLib.Services;
using GeoLib.WindowsHost.Services;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Windows;

namespace GeoLib.WindowsHost
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static MainWindow MainUI { get; set; }

		public MainWindow()
		{
			InitializeComponent();

			btnStart.IsEnabled = true;
			btnStop.IsEnabled = false;

			MainUI = this;

			Title = "UI Running on Thread" + Thread.CurrentThread.ManagedThreadId;
		}

		ServiceHost _hostGeoManager = null;
		ServiceHost _hostMessageManager = null;

		private void btnStart_Click(object sender, RoutedEventArgs e)
		{
			_hostGeoManager = new ServiceHost(typeof(GeoManager));
			_hostMessageManager = new ServiceHost(typeof(MessageManager));

			_hostGeoManager.Open();
			_hostMessageManager.Open();

			btnStart.IsEnabled = false;
			btnStop.IsEnabled = true;
		}

		private void btnStop_Click(object sender, RoutedEventArgs e)
		{
			_hostGeoManager.Close();
			_hostMessageManager.Close();

			btnStart.IsEnabled = true;
			btnStop.IsEnabled = false;
		}

		public void ShowMessage(string message)
		{
			int threadId = Thread.CurrentThread.ManagedThreadId;

			lblMessage.Content = message + Environment.NewLine +
				"(Shown on thread " + Thread.CurrentThread.ManagedThreadId.ToString() + " | Process " + Process.GetCurrentProcess().Id.ToString() + ")";
		}
	}
}

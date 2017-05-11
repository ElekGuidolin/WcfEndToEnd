using GeoLib.Services;
using GeoLib.WindowsHost.Contracts;
using GeoLib.WindowsHost.Services;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Windows;

namespace GeoLib.WindowsHost
{
	/// <summary>
	/// Interaction logic for HostWindow.xaml
	/// </summary>
	public partial class HostWindow : Window
	{
		public static HostWindow MainUI { get; set; }

		ServiceHost _hostGeoManager = null;
		ServiceHost _hostMessageManager = null;
		SynchronizationContext _SyncContext = null;

		public HostWindow()
		{
			InitializeComponent();

			btnStart.IsEnabled = true;
			btnStop.IsEnabled = false;

			MainUI = this;

			Title = "UI Running on Thread: " + Thread.CurrentThread.ManagedThreadId + " | Process: " + Process.GetCurrentProcess().Id.ToString();
			_SyncContext = SynchronizationContext.Current;
		}

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

			SendOrPostCallback callback = new SendOrPostCallback(arg =>
			{
				lblMessage.Content = message + Environment.NewLine +
					"(Marshalled from thread: " + threadId + " to thread " + Thread.CurrentThread.ManagedThreadId.ToString() +
					" | Process: " + Process.GetCurrentProcess().Id.ToString() + ")";
			});

			_SyncContext.Send(callback, null);
		}

		private void btnInProcService_Click(object sender, RoutedEventArgs e)
		{
			Thread thread = new Thread(() =>
			{
				ChannelFactory<IMessageService> factory = new ChannelFactory<IMessageService>("");

				IMessageService proxy = factory.CreateChannel();

				proxy.ShowMessage(DateTime.Now.ToLongTimeString() + " from in-process call.");

				factory.Close();
			})
			{
				IsBackground = true
			};

			thread.Start();
		}
	}
}

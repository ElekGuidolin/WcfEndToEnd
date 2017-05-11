using GeoLib.WindowsHost.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib.WindowsHost.Services
{
	[ServiceBehavior(UseSynchronizationContext = false)]
	public class MessageManager : IMessageService
	{
		public void ShowMessage(string message)
		{
            HostWindow.MainUI.ShowMessage(message + " | Process: " + Process.GetCurrentProcess().Id.ToString());
		}
	}
}

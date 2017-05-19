using System.ServiceModel;

namespace GeoLib.WindowsHost.Contracts
{
	[ServiceContract]
	public interface IMessageService
	{
		[OperationContract]
		void ShowMessage(string message);
	}
}

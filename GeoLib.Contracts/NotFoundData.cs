using System.Runtime.Serialization;

namespace GeoLib.Contracts
{
	[DataContract]
	public class NotFoundData
	{
		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public string When { get; set; }

		[DataMember]
		public string User { get; set; }

	}
}

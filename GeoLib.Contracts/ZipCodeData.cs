using System.Runtime.Serialization;

namespace GeoLib.Contracts
{
	[DataContract(Namespace = "http://smartdev.com.br/ElekGuidolin/WcfEndToEnd")]
	public class ZipCodeData : IExtensibleDataObject
	{
		[DataMember]
		public string City { get; set; }
		[DataMember]
		public string State { get; set; }
		[DataMember]
		public string ZipCode { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}

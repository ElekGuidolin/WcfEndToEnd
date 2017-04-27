using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GeoLib.Data;
using GeoLib.Contracts;
using GeoLib.Services;

namespace GeoLib.Tests
{
	[TestClass]
	public class ManagerTests
	{
		[TestMethod]
		public void Test_Zip_Code_Retrieval()
		{
			//Arrange
			Mock<IZipCodeRepository> mockZipCodeRepository = new Mock<IZipCodeRepository>();

			ZipCode zipCode = new ZipCode()
			{
				City = "LINCOLN PARK",
				State = new State() { Abbreviation = "NJ" },
				Zip = "07035"
			};

			mockZipCodeRepository.Setup(obj => obj.GetByZip("07035")).Returns(zipCode);

			//Act
			IGeoService geoService = new GeoManager(mockZipCodeRepository.Object);
			ZipCodeData data = geoService.GetZipInfo("07035");

			//Assert
			Assert.IsTrue(data.City.ToUpper() == "LINCOLN PARK");
			Assert.AreEqual(data.State, "NJ");
		}
	}
}

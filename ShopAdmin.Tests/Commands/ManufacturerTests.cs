using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using ShopAdmin.Commands;
using ShopGeneral.Data;
using ShopGeneral.Services;
using Manufacturer = ShopAdmin.Commands.Manufacturer;


namespace ShopAdmin.Tests.Commands
{
    [TestClass]
    public class ManufacturerTests
    {
        private Manufacturer sut;
        private readonly Mock<ILogger<Manufacturer>> _logger;
        private readonly Mock<IManufacturerService> _manufacturerService;
        private readonly Mock<IEmailService> _emailService;


        public ManufacturerTests()
        {
            _logger = new Mock<ILogger<Manufacturer>>();
            _manufacturerService = new Mock<IManufacturerService>();
            _emailService = new Mock<IEmailService>();
            sut = new Manufacturer(_logger.Object, _manufacturerService.Object, _emailService.Object);
        }

        [TestMethod]
        public void When_send_date_is_today_sends_email()
        {
            var testDate = DateTime.Today.Day;

            sut.SendReport(testDate);

            _manufacturerService.Verify(service => service.GetAllManufacturers(), Times.Once);
        }

        [TestMethod]
        public void When_send_date_is_not_today_doesnt_send_email()
        {
            var testDate = DateTime.Today.Day + 1;

            sut.SendReport(testDate);

            _manufacturerService.Verify(service => service.GetAllManufacturers(), Times.Never);
        }

        [TestMethod]
        public void When_sending_email_to_manufacturer_name_and_email_is_correct()
        {
            //Arrange
            ShopGeneral.Data.Manufacturer manufacturer = new ShopGeneral.Data.Manufacturer()
            {
                Name = "Test",
                EmailReport = "info@bugatti.se"
            };
            List<ShopGeneral.Data.Manufacturer> testList = new(){manufacturer};
            _manufacturerService.Setup(service => service.GetAllManufacturers()).Returns(testList);
            _emailService.Setup(service => service.IsValidEmail(manufacturer.EmailReport)).Returns(true);

            //Act
            sut.SendReport(DateTime.Today.Day);

            //Assert
            _emailService.Verify(service => service.IsValidEmail(manufacturer.EmailReport), Times.Once);
            _emailService.Verify(service => service.SendMessage(manufacturer.Name, manufacturer.EmailReport, 
                It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void When_incorrect_email_report_email_should_not_be_sent()
        {
            //Arrange
            ShopGeneral.Data.Manufacturer manufacturer = new ShopGeneral.Data.Manufacturer()
            {
                Name = "Test",
                EmailReport = "info@bugatti.se"
            };
            List<ShopGeneral.Data.Manufacturer> testList = new() { manufacturer };
            _manufacturerService.Setup(service => service.GetAllManufacturers()).Returns(testList);
            _emailService.Setup(service => service.IsValidEmail(manufacturer.EmailReport)).Returns(false);
            
            //Act
            sut.SendReport(DateTime.Today.Day);

            //Assert
            _emailService.Verify(service => service.IsValidEmail(manufacturer.EmailReport), Times.Once);
            _emailService.Verify(service => service.SendMessage(manufacturer.Name, manufacturer.EmailReport,
                It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}

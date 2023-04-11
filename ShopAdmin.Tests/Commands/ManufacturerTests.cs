using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using ShopAdmin.Commands;
using ShopGeneral.Data;
using ShopGeneral.Model;
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
            _emailService.Setup(service => service.SendMessages(It.IsAny<List<IEmailInfo>>())).Returns(new List<IEmailInfo>());

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
            _emailService.Setup(service => service.SendMessages(It.IsAny<List<IEmailInfo>>())).Returns(new List<IEmailInfo>());



            //Act
            sut.SendReport(DateTime.Today.Day);

            //Assert
            //Moq headache - verify that the test sends a list of emails where the first email matches the manufacturer we sent in
            _emailService.Verify(service => service.SendMessages(
                It.Is<List<IEmailInfo>>(emails => 
                    string.Equals(emails.First().ReceiverName, manufacturer.Name)
                    && string.Equals(emails.First().ReceiverEmail, manufacturer.EmailReport))), Times.Once);
        }

        [TestMethod]
        public void When_incorrect_email_report_email_should_not_be_sent()
        {
            //Arrange
            ShopGeneral.Data.Manufacturer manufacturer = new ShopGeneral.Data.Manufacturer()
            {
                Name = "Test",
                EmailReport = "info@b  ugatti.se"
            };
            List<ShopGeneral.Data.Manufacturer> testList = new() { manufacturer };
            _manufacturerService.Setup(service => service.GetAllManufacturers()).Returns(testList);
            
            //Act
            sut.SendReport(DateTime.Today.Day);

            //Assert
            _emailService.Verify(service => service.SendMessages(
                It.Is<List<IEmailInfo>>(emails => emails.Count == 0)), Times.Once);
        }

        [TestMethod]
        public void When_sending_several_emails_all_emails_are_sent()
        {
            //Arrange
            ShopGeneral.Data.Manufacturer manufacturer = new ShopGeneral.Data.Manufacturer()
            {
                Name = "Test",
                EmailReport = "info@bugatti.se"
            };

            List<ShopGeneral.Data.Manufacturer> testList = new()
            {
                manufacturer,
                manufacturer,
                manufacturer
            };
            _manufacturerService.Setup(service => service.GetAllManufacturers()).Returns(testList);
            _emailService.Setup(service => service.SendMessages(It.IsAny<List<IEmailInfo>>())).Returns(new List<IEmailInfo>());

            //Act
            sut.SendReport(DateTime.Today.Day);

            //Assert
            _emailService.Verify(service => service.SendMessages(
                It.Is<List<IEmailInfo>>(emails => emails.Count == 3)), Times.Once);
        }

        [TestMethod]
        public void When_sending_several_emails_all_invalid_emails_are_not_sent()
        {
            //Arrange
            ShopGeneral.Data.Manufacturer correct = new ShopGeneral.Data.Manufacturer()
            {
                Name = "Test",
                EmailReport = "info@bugatti.se"
            };

            ShopGeneral.Data.Manufacturer incorrect = new ShopGeneral.Data.Manufacturer()
            {
                Name = "Test",
                EmailReport = "infobuga  tti.se"
            };

            List<ShopGeneral.Data.Manufacturer> testList = new()
            {
                correct,
                correct,
                incorrect
            };
            _manufacturerService.Setup(service => service.GetAllManufacturers()).Returns(testList);
            _emailService.Setup(service => service.SendMessages(It.IsAny<List<IEmailInfo>>())).Returns(new List<IEmailInfo>());

            //Act
            sut.SendReport(DateTime.Today.Day);

            //Assert
            _emailService.Verify(service => service.SendMessages(
                It.Is<List<IEmailInfo>>(emails => emails.Count == 2)), Times.Once);
        }

        [TestMethod]
        public void When_emails_failed_sending_it_is_logged()
        {
            //Arrange
            ShopGeneral.Data.Manufacturer manufacturer = new ShopGeneral.Data.Manufacturer()
            {
                Name = "Test",
                EmailReport = "info@bugatti.se"
            };
            

            List<ShopGeneral.Data.Manufacturer> testList = new()
            {
                manufacturer,
                manufacturer,
            };

            _manufacturerService.Setup(service => service.GetAllManufacturers()).Returns(testList);
            _emailService.Setup(service => service.SendMessages(It.IsAny<List<IEmailInfo>>())).Returns(new List<IEmailInfo>());

            //Act
            sut.SendReport(DateTime.Today.Day);

            //Assert
            _logger.Verify(
            logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        }
    }
}

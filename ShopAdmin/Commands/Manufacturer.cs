using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShopGeneral.Services;

namespace ShopAdmin.Commands
{
    public class Manufacturer : ConsoleAppBase
    {
        private readonly ILogger<Manufacturer> _logger;
        private readonly IManufacturerService _manufacturerService;
        private readonly IEmailService _emailService;

        public Manufacturer(ILogger<Manufacturer> logger, IManufacturerService manufacturerService, IEmailService emailService)
        {
            _logger = logger;
            _manufacturerService = manufacturerService;
            _emailService = emailService;
        }

        public void SendReport(int sendOnDate)
        {
            _logger.LogInformation("SendReport starting.");
            if (sendOnDate != DateTime.Today.Day)
                return;
            
            var manufacturers = _manufacturerService.GetAllManufacturers();
            
            foreach (var manufacturer in manufacturers)
            {
                SendManufacturerReport(manufacturer);
            }

            _logger.LogInformation("SendReport ending.");
        }

        private void SendManufacturerReport(ShopGeneral.Data.Manufacturer manufacturer)
        {
            string name = manufacturer.Name;
            string email = manufacturer.EmailReport;

            if (_emailService.IsValidEmail(email) == false)
            {
                return;
            }

            string report = "";

            _emailService.SendMessage(name, email, "Försäljningsrapport", report);
        }
    }
}

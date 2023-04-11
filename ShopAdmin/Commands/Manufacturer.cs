using Microsoft.Extensions.Logging;
using ShopGeneral.Model;
using ShopGeneral.Services;
using System.Text;

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

        public void SendReport(int sendOnDay)
        {
            _logger.LogInformation("SendReport starting.");

            if (sendOnDay != DateTime.Today.Day)
                return;

            var manufacturers = _manufacturerService.GetAllManufacturers();
            List<IEmailInfo> emails = new List<IEmailInfo>();

            foreach (var manufacturer in manufacturers)
            {
                var email = CreateReportEmail(manufacturer);
                if (email.IsValid())
                {
                    emails.Add(email);
                }
            }

            var sentEmails = _emailService.SendMessages(emails);
            LogUnsentEmails(emails, sentEmails);

            _logger.LogInformation("SendReport ending.");
        }

        private IEmailInfo CreateReportEmail(ShopGeneral.Data.Manufacturer manufacturer)
        {
            string name = manufacturer.Name;
            string email = manufacturer.EmailReport;
            string report = "";
            EmailInfo emailInfo = new EmailInfo(name, email, "Försäljningsrapport", report);
            return emailInfo;
        }
        
        private void LogUnsentEmails(List<IEmailInfo> allEmails, List<IEmailInfo> sentEmails)
        {
            var unsentEmails = allEmails.Where(email => !sentEmails.Contains(email)).ToList();
            if (unsentEmails.Count == 0)
                return;

            var unsentEmailMessage = new StringBuilder();
            unsentEmailMessage.AppendLine("Unsent Emails");
            foreach (var unsentEmail in unsentEmails)
            {
                unsentEmailMessage.AppendLine($"{unsentEmail.ReceiverName} - {unsentEmail.ReceiverEmail}");
            }

            unsentEmailMessage.AppendLine($"Total number of unsent emails {unsentEmails.Count}/{allEmails.Count}");

            _logger.LogWarning(unsentEmailMessage.ToString());
        }
    }
}
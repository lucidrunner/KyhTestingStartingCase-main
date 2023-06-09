﻿using Microsoft.Extensions.Logging;
using ShopGeneral.Model;
using ShopGeneral.Services;
using System.Text;

namespace ShopAdmin.Commands;

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
        {
            _logger.LogInformation("SendReport ending.");
            return;
        }

        var manufacturers = _manufacturerService.GetAllManufacturers();
        var createdEmails = new List<IEmailMessage>();

        foreach (var manufacturer in manufacturers)
        {
            var email = CreateReportEmail(manufacturer);
            if (email.IsValid())
            {
                createdEmails.Add(email);
            }
        }

        var sentEmails = _emailService.SendMessages(createdEmails);
        LogUnsentEmails(createdEmails, sentEmails.ToList());

        _logger.LogInformation("SendReport ending.");
    }

    private IEmailMessage CreateReportEmail(ShopGeneral.Data.Manufacturer manufacturer)
    {
        string name = manufacturer.Name;
        string address = manufacturer.EmailReport;
        string report = "";

        var reportEmail = new EmailMessage(name, address, "Försäljningsrapport", report);
        return reportEmail;
    }

    private void LogUnsentEmails(List<IEmailMessage> allEmails, List<IEmailMessage> sentEmails)
    {
        var unsentEmails = allEmails.Where(email => !sentEmails.Contains(email)).ToList();
        if (unsentEmails.Count == 0)
            return;

        var unsentEmailMessage = new StringBuilder();
        unsentEmailMessage.AppendLine("Unsent Emails");
        foreach (var unsentEmail in unsentEmails)
        {
            unsentEmailMessage.AppendLine($"{unsentEmail.ReceiverName} - {unsentEmail.ReceiverEmailAddress}");
        }

        unsentEmailMessage.AppendLine($"Total number of unsent emails {unsentEmails.Count}/{allEmails.Count}");

        _logger.LogWarning(unsentEmailMessage.ToString());
    }
}
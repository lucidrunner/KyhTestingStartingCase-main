using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using ShopGeneral.Model;

namespace ShopGeneral.Services;

public class EmailService : IEmailService
{
    private readonly SmtpClient _client = new();
    private string? _userName;
    private string? _userEmail;
    private string? _password;
    private string? _serverHost;
    private int _port;
    private readonly bool _validConnection;

    private const string ConfigurationFilePath = "emailconnection.json";

    public EmailService()
    {
        if (!File.Exists(ConfigurationFilePath))
        {
            _validConnection = false;
            return;
        }

        _validConnection = SetupServerConnection();

    }

    private bool SetupServerConnection()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(ConfigurationFilePath)
            .Build();

        _userName = configuration.GetSection("EmailSettings")["UserName"] ?? string.Empty;
        _userEmail = configuration.GetSection("EmailSettings")["UserEmail"] ?? string.Empty;
        _password = configuration.GetSection("EmailSettings")["Password"] ?? string.Empty;
        _serverHost = configuration.GetSection("EmailSettings")["Serverhost"] ?? string.Empty;
        string? port = configuration.GetSection("EmailSettings")["Port"];

        if (_serverHost == string.Empty || _userName == string.Empty || _userEmail == string.Empty ||
            !int.TryParse(port, out _port))
        {
            return false;
        }

        return MailboxAddress.TryParse(_userEmail, out _);
    }

    private bool Connect()
    {
        if (!_validConnection)
            return false;

        try
        {
            _client.Connect(_serverHost, _port, false);
            if (_password != null)
                _client.Authenticate(_userEmail, _password);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void Disconnect()
    {
        _client.Disconnect(true);
    }

    public IEnumerable<IEmailMessage> SendMessages(IEnumerable<IEmailMessage> emailsToSend)
    {
        if (!Connect())
        {
            return new List<IEmailMessage>();
        }

        var sentEmails = new List<IEmailMessage>();

        foreach (var emailInfo in emailsToSend)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_userName, _userEmail));
                email.To.Add(new MailboxAddress(emailInfo.ReceiverName, emailInfo.ReceiverEmailAddress));
                email.Subject = emailInfo.Subject;
                email.Body = new TextPart("plain")
                {
                    Text = emailInfo.Message
                };

                _client.Send(email);
                sentEmails.Add(emailInfo);
            }
            catch
            {
                continue;
            }
        }

        Disconnect();

        return sentEmails;
    }
        
}
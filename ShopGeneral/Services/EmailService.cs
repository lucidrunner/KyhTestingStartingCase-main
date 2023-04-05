using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using ShopGeneral.Model;

namespace ShopGeneral.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _client = new SmtpClient();
        private readonly string _userName;
        private readonly string _userEmail;
        private readonly string _password;
        private readonly string _serverhost;
        private readonly int _port;
        private readonly bool _validConnection;

        public EmailService()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("emailconnection.json")
                .Build();
            
            _userName = configuration.GetSection("EmailSettings")["UserName"] ?? string.Empty;
            _userEmail = configuration.GetSection("EmailSettings")["UserEmail"] ?? string.Empty;
            _password = configuration.GetSection("EmailSettings")["Password"] ?? string.Empty;
            _serverhost = configuration.GetSection("EmailSettings")["Serverhost"] ?? string.Empty;
            var port = configuration.GetSection("EmailSettings")["Port"];

            if (_serverhost == string.Empty || _userName == string.Empty || _userEmail == string.Empty ||
                !int.TryParse(port, out _port))
            {
                _validConnection = false;
                return;
            }
            if (!MailboxAddress.TryParse(_userEmail, out MailboxAddress _))
            {
                _validConnection = false;
                return;
            }

            _validConnection = true;
        }

        private bool Connect()
        {
            if (!_validConnection)
                return false;

            try
            {
                _client.Connect(_serverhost, _port, false);
                if (_password != null)
                    _client.Authenticate(_userEmail, _password);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private void Disconnect()
        {
            _client.Disconnect(true);
        }

        public List<IEmailInfo> SendMessages(List<IEmailInfo> emailsToSend)
        {
            if (!Connect())
            {
                return new List<IEmailInfo>();
            }

            var sentEmails = new List<IEmailInfo>();

            foreach (IEmailInfo emailInfo in emailsToSend)
            {
                try
                {
                    var email = new MimeMessage();
                    email.From.Add(new MailboxAddress(_userName, _userEmail));
                    email.To.Add(new MailboxAddress(emailInfo.ReceiverName, emailInfo.ReceiverEmail));
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
}

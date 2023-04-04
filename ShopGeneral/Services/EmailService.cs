using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using ShopGeneral.Model;

namespace ShopGeneral.Services
{
    public class EmailService : IEmailService
    {
        private SmtpClient _client = new SmtpClient();
        //TODO Ladda från config istället
        private string _userName;
        private string _userEmail;
        private string _password;
        private string _serverhost;
        private int _port = 587;

        public EmailService()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("emailconnection.json")
                .Build();
            
            //TODO Hur vill man felhantera detta?
            _userName = configuration.GetSection("EmailSettings")["UserName"];
            _userEmail = configuration.GetSection("EmailSettings")["UserEmail"];
            _password = configuration.GetSection("EmailSettings")["Password"];
            _serverhost = configuration.GetSection("EmailSettings")["Serverhost"];
            var port = configuration.GetSection("EmailSettings")["Port"];

            if (!int.TryParse(port, out _port))
            {
                //?
            }
            
        }

        private void Connect()
        {
            _client.Connect(_serverhost, _port, false);
            _client.Authenticate(_userEmail, _password);
        }

        private void Disconnect()
        {
            _client.Disconnect(true);
        }

        public void SendMessage(IEmailInfo emailInfo)
        {
            SendMessages(new List<IEmailInfo>(){emailInfo});
        }

        public void SendMessages(List<IEmailInfo> emailInfos)
        {
            Connect();

            foreach (IEmailInfo emailInfo in emailInfos)
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
            }

            Disconnect();
        }
        
    }
}

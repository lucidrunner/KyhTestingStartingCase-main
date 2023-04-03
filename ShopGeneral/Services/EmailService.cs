using System.Runtime.InteropServices.ComTypes;
using MailKit.Net.Smtp;
using MimeKit;

namespace ShopGeneral.Services
{
    public class EmailService : IEmailService
    {
        //TODO Ladda från config istället
        private const string UserName = "Savanah Larson";
        private const string UserEmail = "savanah.larson32@ethereal.email";
        private const string Password = "4mwQ6gtQUS4fKPrF92";
        private const string Serverhost = "smtp.ethereal.email";
        private const int Port = 587;

        public bool IsValidEmail(string email)
        {
            return email != null && MailboxAddress.TryParse(email, out MailboxAddress _);
        }

        //private SmtpClient _client = null;

        //public void Connect()
        //{
        //    _client = new SmtpClient();
        //    _client.Connect(Serverhost, Port, false);
        //    _client.Authenticate(UserEmail, Password);
        //}

        //public void Disconnect()
        //{
        //    _client.Disconnect(true);
        //    _client = null;
        //}

        public void SendMessage(string nameTo, string emailTo, string subject, string message)
        {
            //if (_client == null)
            //    Connect();

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(UserName, UserEmail));
            email.To.Add(new MailboxAddress(nameTo, emailTo));
            email.Subject = subject;
            email.Body = new TextPart("plain")
            {
                Text = message
            };


            using (var client = new SmtpClient())
            {
                client.Connect(Serverhost, Port, false);

                client.Authenticate(UserEmail, Password);

                client.Send(email);
                client.Disconnect(true);
            }
        }

        //TODO Gör detta
        //public void SendMessages(List<EmailInfo> emails)
        //{
        //    //Anslut till clienten

        //    //Foreach
        //    //För varje info i emails, skapa en ny Mimemessage
        //    //Skicka meddelandet
            
        //    //Disconnecta på slutet
        //}
    }
}

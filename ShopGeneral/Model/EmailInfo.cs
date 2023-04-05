using MimeKit;

namespace ShopGeneral.Model;

public class EmailInfo : IEmailInfo
{
    public string ReceiverName { get; }
    public string ReceiverEmail { get; }

    public string Subject { get;  }

    public string Message { get;  }

    public EmailInfo(string receiverName, string receiverEmail, string subject, string message)
    {
        ReceiverName = receiverName;
        ReceiverEmail = receiverEmail;
        Subject = subject;
        Message = message;
    }

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(ReceiverName))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(ReceiverEmail) || !MailboxAddress.TryParse(ReceiverEmail, out MailboxAddress _))
        {
            return false;
        }

        return true;
    }
}
using MimeKit;

namespace ShopGeneral.Model;

public class EmailMessage : IEmailMessage
{
    public string ReceiverName { get; }
    public string ReceiverEmailAddress { get; }

    public string Subject { get;  }

    public string Message { get;  }

    public EmailMessage(string receiverName, string receiverEmailAddress, string subject, string message)
    {
        ReceiverName = receiverName;
        ReceiverEmailAddress = receiverEmailAddress;
        Subject = subject;
        Message = message;
    }

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(ReceiverName))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(ReceiverEmailAddress) || !MailboxAddress.TryParse(ReceiverEmailAddress, out _))
        {
            return false;
        }

        return true;
    }
}
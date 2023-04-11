using ShopGeneral.Model;

namespace ShopGeneral.Services;

public interface IEmailService
{
    IEnumerable<IEmailMessage> SendMessages(IEnumerable<IEmailMessage> emailsToSend);

}
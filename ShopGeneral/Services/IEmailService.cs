using ShopGeneral.Model;

namespace ShopGeneral.Services;

public interface IEmailService
{
    List<IEmailMessage> SendMessages(List<IEmailMessage> emailsToSend);

}
using ShopGeneral.Model;

namespace ShopGeneral.Services;

public interface IEmailService
{
    List<IEmailInfo> SendMessages(List<IEmailInfo> emailsToSend);

}
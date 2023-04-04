using ShopGeneral.Model;

namespace ShopGeneral.Services;

public interface IEmailService
{
    void SendMessage(IEmailInfo emailInfo);
    void SendMessages(List<IEmailInfo> emailInfos);

}
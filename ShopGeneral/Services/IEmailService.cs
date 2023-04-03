namespace ShopGeneral.Services;

public interface IEmailService
{
    bool IsValidEmail(string email);
    void SendMessage(string nameTo, string emailTo, string subject, string message);

    //void Connect();
    //void Disconnect();
}
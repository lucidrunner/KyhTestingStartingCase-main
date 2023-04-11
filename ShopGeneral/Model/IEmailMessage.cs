namespace ShopGeneral.Model;

public interface IEmailMessage
{
    public string ReceiverName { get; }
    public string ReceiverEmailAddress { get; }

    public string Subject { get; }

    public string Message { get; }

    public bool IsValid();
}
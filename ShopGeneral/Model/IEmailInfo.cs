namespace ShopGeneral.Model;

public interface IEmailInfo
{
    public string ReceiverName { get; }
    public string ReceiverEmail { get; }

    public string Subject { get; }

    public string Message { get; }

    public bool IsValid();
}
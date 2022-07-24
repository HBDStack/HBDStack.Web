namespace HBDStack.Web.Auths;

public sealed class StatusResult
{
    public bool IsSuccess { get; }
    public string? Message { get; }

    private StatusResult(bool isSuccess, string? message = null)
    {
        IsSuccess = isSuccess;
        Message = message;

        if (!IsSuccess && string.IsNullOrEmpty(Message))
            throw new ArgumentNullException(nameof(message));
    }

    public static StatusResult Success() => new StatusResult(true);
    public static StatusResult Fails(string message) => new StatusResult(false, message);

}
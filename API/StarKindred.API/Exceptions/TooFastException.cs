namespace StarKindred.API.Exceptions;

public class TooFastException: AppException
{
    public int RetryInSeconds { get; }

    public TooFastException(string message, int retryInSeconds)
        : base(420, $"{message} Try again in {retryInSeconds} {(retryInSeconds == 1 ? "second" : "seconds")}.")
    {
        RetryInSeconds = retryInSeconds;
    }
}
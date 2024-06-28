namespace StarKindred.API.Exceptions;

public class NotLoggedInException: AppException
{
    public NotLoggedInException(string message) : base(401, message)
    {
    }
}
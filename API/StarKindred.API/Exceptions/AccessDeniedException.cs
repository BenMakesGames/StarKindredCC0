namespace StarKindred.API.Exceptions;

public class AccessDeniedException: AppException
{
    public AccessDeniedException(string message) : base(403, message)
    {
    }
}
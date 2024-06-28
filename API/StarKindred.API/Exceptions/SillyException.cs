namespace StarKindred.API.Exceptions;

public class SillyException: AppException
{
    public SillyException(string message) : base(490, message)
    {
    }
}
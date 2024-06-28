namespace StarKindred.API.Exceptions;

public class NotFoundException: AppException
{
    public NotFoundException(string message) : base(404, message)
    {
    }
}
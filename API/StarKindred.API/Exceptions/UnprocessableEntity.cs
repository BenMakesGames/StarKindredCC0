namespace StarKindred.API.Exceptions;

public class UnprocessableEntity: AppException
{
    public UnprocessableEntity(string message) : base(422, message)
    {
    }
}
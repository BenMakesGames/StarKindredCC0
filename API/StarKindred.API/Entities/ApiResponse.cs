namespace StarKindred.API.Entities;

public record ApiResponse
{
    public List<ApiMessage> Messages { get; init; } = new();
}

public sealed record ApiResponse<TDto>(TDto Data): ApiResponse;

public sealed record ApiMessage(MessageType Type, string Text)
{
    public static ApiMessage Info(string text) => new(MessageType.Info, text);
    public static ApiMessage Error(string text) => new(MessageType.Error, text);
    public static ApiMessage Success(string text) => new(MessageType.Success, text);
}

public enum MessageType
{
    Info,
    Error,
    Success,
}
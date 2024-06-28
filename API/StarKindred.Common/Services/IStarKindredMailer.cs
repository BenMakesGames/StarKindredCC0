namespace StarKindred.Common.Services;

public interface IStarKindredMailer
{
    public Task SendEmailAsync(string to, string subject, string messagePlainText, string messageHtml, CancellationToken cToken);
}
using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using Microsoft.Extensions.Configuration;
using StarKindred.Common.Services;

namespace StarKindred.AzureMailer.Services;

public class AzureMailer: IStarKindredMailer
{
    private string SenderAddress { get; }
    private EmailClient EmailClient { get; }

    public AzureMailer(IConfiguration configuration)
    {
        string connectionString = configuration.GetSection("AzureMailer:ConnectionString").Get<string?>()
            ?? throw new Exception("AzureMailer:ConnectionString is not present in app configuration.");

        SenderAddress = configuration.GetSection("AzureMailer:SenderAddress").Get<string?>()
            ?? throw new Exception("AzureMailer:SenderAddress is not present in app configuration.");

        EmailClient = new EmailClient(connectionString);
    }

    public async Task SendEmailAsync(string to, string subject, string messagePlainText, string messageHtml, CancellationToken cToken)
    {
        var emailContent = new EmailContent(subject)
        {
            Html = messageHtml,
            PlainText = messagePlainText
        };

        var recipients = new EmailRecipients(new List<EmailAddress>() { new(to) });

        var emailMessage = new EmailMessage(SenderAddress, emailContent, recipients);

        await EmailClient.SendAsync(emailMessage, cToken);
    }
}
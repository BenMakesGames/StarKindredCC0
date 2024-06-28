using System.Security.Claims;
using JNogueira.Logger.Discord;

namespace StarKindred.API.Configuration;

public static class DiscordLogging
{
    public static WebApplicationBuilder AddDiscordLogging(this WebApplicationBuilder builder)
    {
        var discordLogging = builder.Configuration.GetSection("DiscordLogger").Get<DiscordLogger>();

        if(discordLogging == null)
            return builder;
        
        builder.Logging.AddProvider(
            new DiscordLoggerProvider(
                new DiscordLoggerOptions(discordLogging.WebhookUrl)
                {
                    ApplicationName = "StarKindred",
                    EnvironmentName = discordLogging.EnvironmentName,
                    UserName = discordLogging.BotName,
                    UserClaimValueToDiscordFields = new List<UserClaimValueToDiscordField>
                    {
                        new(ClaimTypes.NameIdentifier, "Name identifier"),
                        new(ClaimTypes.Name, "Name")
                    }
                }
            )
        );

        return builder;
    }

    public class DiscordLogger
    {
        public string WebhookUrl { get; init; } = null!;
        public string EnvironmentName { get; init; } = null!;
        public string BotName { get; init; } = null!;
    }
}
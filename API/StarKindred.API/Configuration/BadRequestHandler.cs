using System.Text.Json;
using StarKindred.API.Entities;

namespace StarKindred.API.Configuration;

public static class BadRequestHandler
{
    public static IApplicationBuilder UseBadRequestHandler(this WebApplication app, JsonSerializerOptions jsonSerializationOptions)
    {
        return app.Use(async (context, next) => {
            await next();

            if (
                (context.Response.StatusCode is 404 && context.Response.Headers.Count == 0) ||
                context.Response.StatusCode == 405
            )
            {
                var message = context.Response.StatusCode switch
                {
                    404 => "Not found.",
                    405 => "Method not allowed.",
                    _ => "Unknown Error"
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new ApiResponse()
                {
                    Messages = new() { ApiMessage.Error(message) }
                }, jsonSerializationOptions));
            }
        });
    }
}
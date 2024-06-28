namespace StarKindred.API.Configuration;

public static class HeartbeatHandler
{
    public static IApplicationBuilder UseHeartbeatHandler(this WebApplication app, string url)
    {
        app.MapGet(url, async context =>
        {
            context.Response.Headers.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync("ドキドキ");
        });

        return app;
    }
}
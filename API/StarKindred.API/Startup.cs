using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using EFCoreSecondLevelCacheInterceptor;
using FluentValidation;
using FluentValidation.AspNetCore;
using StarKindred.API.Configuration;
using StarKindred.API.Middleware;
using StarKindred.API.Services;
using StarKindred.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarKindred.AzureMailer.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:ApiKey").Get<string>()
    ?? throw new Exception("Stripe:ApiKey is missing from configuration.");

builder.AddDiscordLogging();

var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>()
    ?? throw new Exception("CORS:AllowedOrigins is missing from configuration.");

builder.Services.AddCors(options =>
{
    options.AddPolicy("StarKindred", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
        ;
    });
});

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<InvalidModelStateFilter>();
        options.Filters.Add<AppExceptionFilter>();
        options.RespectBrowserAcceptHeader = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
;

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Singleton);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// TODO: when we have in-memory tables in the DB, probably get rid of EFSecondLevelCache??
builder.Services.AddEFSecondLevelCache(options => options
    .UseMemoryCacheProvider()
    .DisableLogging(true)
);

var mysqlConnectionString = builder.Configuration.GetConnectionString("Db")
        ?? throw new Exception("Db is missing from configuration.");

var mysqlServerVersion = ServerVersion.AutoDetect(mysqlConnectionString);

builder.Services.AddDbContext<Db>((serviceProvider, options) =>
{
    options
        .UseMySql(mysqlConnectionString, mysqlServerVersion, o =>
        {
            o.EnableRetryOnFailure(3);
        })
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
        .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>())
    ;

    if(builder.Configuration.GetValue<bool>("LogSqlToConsole"))
        options.LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services
    .AddSingleton(new Random())
    .AddSingleton<IPassphraseHasher, PassphraseHasher>()
    .AddScoped<ICurrentUser, CurrentUser>()
    .AddSingleton<IStarKindredMailer, AzureMailer>()
    .AddSingleton<IAddressHelper, AddressHelper>()
    .AddHttpContextAccessor()
;

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("StarKindred");

app.UseAuthorization();

app.MapControllers();

app.UseHeartbeatHandler("/heartbeat");

var jsonSerializationOptions = new JsonSerializerOptions()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Converters = { new JsonStringEnumConverter() }
};

app.UseBadRequestHandler(jsonSerializationOptions);

app.Run();

// ReSharper disable once PartialTypeWithSinglePart
public partial class Startup { } // for tests & benchmarking
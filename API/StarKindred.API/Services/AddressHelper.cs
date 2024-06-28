using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace StarKindred.API.Services;

public interface IAddressHelper
{
    string ServerUrl { get; }
    string ClientUrl { get; }
}

public class AddressHelper: IAddressHelper
{
    public string ServerUrl { get; }
    public string ClientUrl { get; }

    public AddressHelper(IServer server, IConfiguration configuration)
    {
        ServerUrl = configuration.GetSection("APIAddress").Get<string?>()
            ?? server.Features.Get<IServerAddressesFeature>()!.Addresses.First(a => !a.Contains("[::]")) // doesn't work in prod, for some reason; does work for local
            ?? throw new Exception("APIAddress is missing from configuration.");

        ClientUrl = configuration.GetSection("CORS:AllowedOrigins").Get<string[]>()?.FirstOrDefault()
            ?? throw new Exception("CORS:AllowedOrigins is missing from configuration.");
    }
}
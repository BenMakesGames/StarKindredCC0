using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using StarKindred.API.Services;

namespace StarKindred.API.Benchmarks.Endpoints.Vassals;

[MemoryDiagnoser(false)]
public class SearchBenchmark
{
    private HttpClient APIClient { get; }

    public SearchBenchmark()
    {
        var webAppFactory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<ICurrentUser, FixedCurrentUser>();
                });
            })
        ;

        APIClient = webAppFactory.CreateClient();
    }
    
    [Benchmark]
    public async Task Search()
    {
        for(int i = 0; i < 100; i++)
        {
            // ReSharper disable once UnusedVariable
            var result = await APIClient.GetAsync("/vassals/search/noCache");
        }
    }

    [Benchmark]
    public async Task Search_Cached()
    {
        for(int i = 0; i < 100; i++)
        {
            // ReSharper disable once UnusedVariable
            var result = await APIClient.GetAsync("/vassals/search");
        }
    }

    public class FixedCurrentUser: ICurrentUser
    {
        private static readonly Guid ARealUserId = Guid.Parse("08da36e6-c5ef-470c-8ea9-af58c61c8ab0");
        private static readonly DateTimeOffset TheDistantFuture = DateTimeOffset.UtcNow.AddYears(10);

        public Guid? GetSessionId() => ARealUserId;

        public Task<ICurrentUser.CurrentSessionDto?> GetSession(CancellationToken cToken)
            => Task.FromResult((ICurrentUser.CurrentSessionDto?)new ICurrentUser.CurrentSessionDto(
                ARealUserId,
                "Someone",
                TheDistantFuture
            ))
        ;

        public Task ClearSessionOrThrow(CancellationToken cToken) => Task.CompletedTask;
    }
}
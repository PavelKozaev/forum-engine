using Serilog;
using Serilog.Filters;

namespace ForumEngine.API.DependencyInjection
{
    internal static class LoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddApiLogging(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment environment) =>
            services.AddLogging(b => b.AddSerilog(new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithProperty("Application", "ForumEngine.API")
                .Enrich.WithProperty("Environment", environment.EnvironmentName)
                .WriteTo.Logger(lc => lc
                    .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                    .WriteTo.OpenSearch(
                        configuration.GetConnectionString("Logs"),
                        "forum-logs-{0.yyyy.MM.dd}"))
                .WriteTo.Logger(lc => lc
                    .WriteTo.Console())
                .CreateLogger()));


    }
}
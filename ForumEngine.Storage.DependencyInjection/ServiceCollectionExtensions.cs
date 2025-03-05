using ForumEngine.Domain.UseCases.CreateTopic;
using ForumEngine.Domain.UseCases.GetForums;
using ForumEngine.Storage.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ForumEngine.Storage.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddForumStorage(this IServiceCollection services, string dbConnectionString) => 
            services
            .AddScoped<IGetForumsStorage, GetForumsStorage>()
            .AddScoped<ICreateTopicStorage, CreateTopicStorage>()
            .AddScoped<IGuidFactory, GuidFactory>()
            .AddScoped<IMomentProvider, MomentProvider>()
            .AddDbContextPool<ForumDbContext>(options => options
                .UseNpgsql(dbConnectionString));
    }
}
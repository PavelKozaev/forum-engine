using ForumEngine.Domain.UseCases.CreateForum;
using ForumEngine.Domain.UseCases.CreateTopic;
using ForumEngine.Domain.UseCases.GetForums;
using ForumEngine.Domain.UseCases.GetTopics;
using ForumEngine.Storage.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ForumEngine.Domain.UseCases.SignIn;
using ForumEngine.Domain.UseCases.SignOn;

namespace ForumEngine.Storage.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddForumStorage(this IServiceCollection services, string dbConnectionString)
        {
            services
                .AddScoped<ICreateForumStorage, CreateForumStorage>()
                .AddScoped<IGetForumsStorage, GetForumsStorage>()
                .AddScoped<ICreateTopicStorage, CreateTopicStorage>()
                .AddScoped<IGetTopicsStorage, GetTopicsStorage>()
                .AddScoped<ISignOnStorage, SignOnStorage>()
                .AddScoped<ISignInStorage, SignInStorage>()
                .AddScoped<IGuidFactory, GuidFactory>()
                .AddScoped<IMomentProvider, MomentProvider>()
                .AddDbContextPool<ForumDbContext>(options => options
                    .UseNpgsql(dbConnectionString));

            services.AddMemoryCache();
 
            services.AddAutoMapper(config => config
                .AddMaps(Assembly.GetAssembly(typeof(ForumDbContext))));
 
            return services;
        }
    }
}
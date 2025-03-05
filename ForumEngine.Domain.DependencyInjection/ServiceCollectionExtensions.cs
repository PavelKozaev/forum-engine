using FluentValidation;
using ForumEngine.Domain.Authentication;
using ForumEngine.Domain.Authorization;
using ForumEngine.Domain.Models;
using ForumEngine.Domain.UseCases.CreateTopic;
using ForumEngine.Domain.UseCases.GetForums;
using Microsoft.Extensions.DependencyInjection;

namespace ForumEngine.Domain.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddForumDomain(this IServiceCollection services)
        {
            services
                .AddScoped<IGetForumsUseCase, GetForumsUseCase>()
                .AddScoped<ICreateTopicUseCase, CreateTopicUseCase>()
                .AddScoped<IIntentionResolver, TopicIntentionResolver>();
                
            services
                .AddScoped<IIntentionManager, IntentionManager>()
                .AddScoped<IIdentityProvider, IdentityProvider>();

            services.AddValidatorsFromAssemblyContaining<Forum>(includeInternalTypes: true);

            return services;
        }
    }
}
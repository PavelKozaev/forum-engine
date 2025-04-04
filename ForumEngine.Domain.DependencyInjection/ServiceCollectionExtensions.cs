using FluentValidation;
using ForumEngine.Domain.Authentication;
using ForumEngine.Domain.Authorization;
using ForumEngine.Domain.Models;
using ForumEngine.Domain.UseCases.CreateForum;
using ForumEngine.Domain.UseCases.CreateTopic;
using ForumEngine.Domain.UseCases.GetForums;
using ForumEngine.Domain.UseCases.GetTopics;
using ForumEngine.Domain.UseCases.SignIn;
using ForumEngine.Domain.UseCases.SignOn;
using Microsoft.Extensions.DependencyInjection;

namespace ForumEngine.Domain.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddForumDomain(this IServiceCollection services)
        {
            services
                .AddScoped<ICreateForumUseCase, CreateForumUseCase>()
                .AddScoped<IIntentionResolver, ForumIntentionResolver>()
                .AddScoped<IGetForumsUseCase, GetForumsUseCase>()
                .AddScoped<ICreateTopicUseCase, CreateTopicUseCase>()
                .AddScoped<IGetTopicsUseCase, GetTopicsUseCase>()
                .AddScoped<ISignOnUseCase, SignOnUseCase>()
                .AddScoped<ISignInUseCase, SignInUseCase>()
                .AddScoped<IIntentionResolver, TopicIntentionResolver>();
                
            services
                .AddScoped<IIntentionManager, IntentionManager>()
                .AddScoped<IIdentityProvider, IdentityProvider>()
                .AddScoped<IPasswordManager, PasswordManager>()
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<ISymmetricDecryptor, AesSymmetricEncryptorDecryptor>()
                .AddScoped<ISymmetricEncryptor, AesSymmetricEncryptorDecryptor>();

            services.AddValidatorsFromAssemblyContaining<Forum>(includeInternalTypes: true);

            return services;
        }
    }
}
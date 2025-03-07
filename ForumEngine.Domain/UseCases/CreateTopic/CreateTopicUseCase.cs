using ForumEngine.Domain.Authentication;
using ForumEngine.Domain.Authorization;
using FluentValidation;
using ForumEngine.Domain.UseCases.GetForums;

namespace ForumEngine.Domain.UseCases.CreateTopic
{
    internal class CreateTopicUseCase : ICreateTopicUseCase
    {
        private readonly IValidator<CreateTopicCommand> _validator;
        private readonly IIntentionManager _intentionManager;
        private readonly IIdentityProvider _identityProvider;
        private readonly IGetForumsStorage _getForumsStorage;
        private readonly ICreateTopicStorage _storage;

        public CreateTopicUseCase(
            IValidator<CreateTopicCommand> validator,
            IIntentionManager intentionManager,
            IIdentityProvider identityProvider,
            IGetForumsStorage getForumsStorage,
            ICreateTopicStorage storage)
        {
            _validator = validator;
            _intentionManager = intentionManager;
            _identityProvider = identityProvider;
            _getForumsStorage = getForumsStorage;
            _storage = storage;
        }

        public async Task<Models.Topic> Execute(CreateTopicCommand command, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(command, cancellationToken);

            var (forumId, title) = command;
            _intentionManager.ThrowIfForbidden(TopicIntention.Create);

            await _getForumsStorage.ThrowIfForumNotFound(forumId, cancellationToken);

            return await _storage.CreateTopic(forumId, _identityProvider.Current.UserId, title, cancellationToken);   
        }
    }
}
using ForumEngine.Domain.Exceptions;
using ForumEngine.Domain.Authentication;
using ForumEngine.Domain.Authorization;
using FluentValidation;

namespace ForumEngine.Domain.UseCases.CreateTopic
{
    public class CreateTopicUseCase : ICreateTopicUseCase
    {
        private readonly IValidator<CreateTopicCommand> _validator;
        private readonly IIntentionManager _intentionManager;
        private readonly IIdentityProvider _identityProvider;
        private readonly ICreateTopicStorage _storage;

        public CreateTopicUseCase(
            IValidator<CreateTopicCommand> validator,
            IIntentionManager intentionManager,
            IIdentityProvider identityProvider,
            ICreateTopicStorage storage)
        {
            _validator = validator;
            _intentionManager = intentionManager;
            _identityProvider = identityProvider;
            _storage = storage;
        }

        public async Task<Models.Topic> Execute(CreateTopicCommand command, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(command, cancellationToken);

            var (forumId, title) = command;
            _intentionManager.ThrowIfForbidden(TopicIntention.Create);

            var forumExists = await _storage.ForumExists(forumId, cancellationToken);
            if (!forumExists) throw new ForumNotFoundException(forumId);

            return await _storage.CreateTopic(forumId, _identityProvider.Current.UserId, title, cancellationToken);   
        }
    }
}
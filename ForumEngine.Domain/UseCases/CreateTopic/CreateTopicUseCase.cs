using ForumEngine.Domain.Exceptions;
using ForumEngine.Domain.Authentication;
using ForumEngine.Domain.Authorization;

namespace ForumEngine.Domain.UseCases.CreateTopic
{
    public class CreateTopicUseCase : ICreateTopicUseCase
    {
        private readonly IIntentionManager _intentionManager;
        private readonly IIdentityProvider _identityProvider;
        private readonly ICreateTopicStorage _storage;

        public CreateTopicUseCase(
            IIntentionManager intentionManager,
            IIdentityProvider identityProvider,
            ICreateTopicStorage storage)
        {
            _intentionManager = intentionManager;
            _identityProvider = identityProvider;
            _storage = storage;
        }

        public async Task<Models.Topic> Execute(Guid forumId, string title, CancellationToken cancellationToken)
        {
            _intentionManager.ThrowIfForbidden(TopicIntention.Create);

            var forumExists = await _storage.ForumExists(forumId, cancellationToken);
            if (!forumExists) throw new ForumNotFoundException(forumId);

            return await _storage.CreateTopic(forumId, _identityProvider.Current.UserId, title, cancellationToken);   
        }
    }
}
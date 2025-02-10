using ForumEngine.Domain.Authorization;
using ForumEngine.Domain.Authentication;

namespace ForumEngine.Domain.UseCases.CreateTopic
{
    public class TopicIntentionResolver : IIntentionResolver<TopicIntention>
    {
        public bool IsAllowed(IIdentity subject, TopicIntention intention) => intention switch
            {
                TopicIntention.Create => subject.IsAuthenticated(),
                _ => false,
            };
    }
}
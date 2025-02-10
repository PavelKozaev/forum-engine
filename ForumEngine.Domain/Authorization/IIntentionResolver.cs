using ForumEngine.Domain.Authentication;

namespace ForumEngine.Domain.Authorization
{
    public interface IIntentionResolver
    {

    }

    public interface IIntentionResolver<in TIntention> : IIntentionResolver
    {
        bool IsAllowed(IIdentity subject, TIntention intention);
    }
}
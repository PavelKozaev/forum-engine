using ForumEngine.Domain.Authentication;

namespace ForumEngine.Domain.Authorization
{
    public interface IIntentionManager
    {
        bool IsAllowed<TIntention>(TIntention intention) where TIntention : struct;
        bool IsAllowed<TIntention, TObject>(TIntention intention, TObject target) where TIntention : struct;
    }

    internal class IntentionManager : IIntentionManager
    {
        private readonly IEnumerable<IIntentionResolver> _resolvers;
        private readonly IIdentityProvider _identityProvider;

        public IntentionManager(
            IEnumerable<IIntentionResolver> resolvers,
            IIdentityProvider identityProvider)
        {
            _resolvers = resolvers;
            _identityProvider = identityProvider;
        }

        public bool IsAllowed<TIntention>(TIntention intention) where TIntention : struct
        {
            var maychingResolver = _resolvers.OfType<IIntentionResolver<TIntention>>().FirstOrDefault();

            return maychingResolver?.IsAllowed(_identityProvider.Current, intention) ?? false;
        }

        public bool IsAllowed<TIntention, TObject>(TIntention intention, TObject target) where TIntention : struct
        {
            throw new NotImplementedException();
        }
    }

    internal static class IntentionManagerExtentions
    {
        public static void ThrowIfForbidden<TIntention>(this IIntentionManager intentionManager, TIntention intention)
            where TIntention : struct
        {
            if (!intentionManager.IsAllowed(intention))
            {
                throw new IntentionManagerException();
            }
        }
    }
}
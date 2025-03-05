namespace ForumEngine.Domain.Authentication
{
    internal class IdentityProvider : IIdentityProvider
    {
        public IIdentity Current => new User(Guid.Parse("35b3477c-6c44-4717-8c82-8b95677feb84"));
    }
}
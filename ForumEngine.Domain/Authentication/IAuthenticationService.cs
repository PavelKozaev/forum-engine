namespace ForumEngine.Domain.Authentication
{
    public interface IAuthenticationService
    {
        Task<(bool succes, string authToken)> SignIn(BasicSignInCredentials credentials, CancellationToken cancellationToken);
        Task<IIdentity> Authenticate(string authToken, CancellationToken cancellationToken);
    }

}
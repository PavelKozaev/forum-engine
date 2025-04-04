using ForumEngine.Domain.Authentication;

namespace ForumEngine.Domain.UseCases.SignIn
{
    public interface ISignInUseCase
    {
        Task<(IIdentity identity, string token)> Execute(SignInCommand command, CancellationToken cancellationToken);
    }
}
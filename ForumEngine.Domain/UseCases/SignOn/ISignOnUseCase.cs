using ForumEngine.Domain.Authentication;

namespace ForumEngine.Domain.UseCases.SignOn
{
    public interface ISignOnUseCase
    {
        Task<IIdentity> Execute(SignOnCommand command, CancellationToken cancellationToken);
    }
}
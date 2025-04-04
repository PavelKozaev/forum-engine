namespace ForumEngine.Domain.UseCases.SignIn
{
    public interface ISignInStorage
    {
        Task<RecognisedUser?> FindUser(string login, CancellationToken cancellationToken);
    }
}
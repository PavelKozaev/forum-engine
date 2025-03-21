namespace ForumEngine.Domain.Authentication
{
    public interface IAuthenticationStorage
    {
        Task<RecognizedUser?> FindUser(string login, CancellationToken cancellationToken);
    }
}
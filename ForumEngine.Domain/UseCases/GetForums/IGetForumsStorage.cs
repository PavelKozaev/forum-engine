using ForumEngine.Domain.Models;

namespace ForumEngine.Domain.UseCases.GetForums
{
    public interface IGetForumsStorage
    {
        Task<IEnumerable<Forum>> GetForums(CancellationToken cancellationToken);
    }
}
using ForumEngine.Domain.Models;

namespace ForumEngine.Domain.UseCases.GetForums
{
    public interface IGetForumsUseCase
    {
        Task<IEnumerable<Forum>> Execute(CancellationToken cancellationToken);
    }
}
using ForumEngine.Domain.Models;

namespace ForumEngine.Domain.UseCases.CreateForum
{
    public interface ICreateForumUseCase
    {
        Task<Forum> Execute(CreateForumCommand command, CancellationToken cancellationToken);
    }
}
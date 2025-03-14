using ForumEngine.Domain.Models;

namespace ForumEngine.Domain.UseCases.CreateForum
{
    public interface ICreateForumStorage
    {
        public Task<Forum> Create(string title, CancellationToken cancellationToken);
    }
}
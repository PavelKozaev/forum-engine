using ForumEngine.Domain.Models;

namespace ForumEngine.Domain.UseCases.CreateTopic
{
    public interface ICreateTopicUseCase
    {
        Task<Topic> Execute(Guid forumId, string tile, CancellationToken cancellationToken);
    }
}
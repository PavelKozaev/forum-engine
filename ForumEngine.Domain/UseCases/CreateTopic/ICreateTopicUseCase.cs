using ForumEngine.Domain.Models;

namespace ForumEngine.Domain.UseCases.CreateTopic
{
    public interface ICreateTopicUseCase
    {
        Task<Topic> Execute(CreateTopicCommand command, CancellationToken cancellationToken);
    }
}
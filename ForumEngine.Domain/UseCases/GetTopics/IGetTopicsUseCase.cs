using ForumEngine.Domain.Models;

namespace ForumEngine.Domain.UseCases.GetTopics
{
    public interface IGetTopicsUseCase
    {
        Task<(IEnumerable<Topic> resources, int totalCount)> Execute(GetTopicsQuery query, CancellationToken cancellationToken);
    }

}
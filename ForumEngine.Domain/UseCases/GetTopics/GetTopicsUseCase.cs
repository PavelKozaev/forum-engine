using FluentValidation;
using ForumEngine.Domain.Models;

namespace ForumEngine.Domain.UseCases.GetTopics
{
    internal class GetTopicsUseCase : IGetTopicsUseCase
    {
        private readonly IValidator<GetTopicsQuery> _validator;
        private readonly IGetTopicsStorage _storage;

        public GetTopicsUseCase(
            IValidator<GetTopicsQuery> validator,
            IGetTopicsStorage storage)
        {
            _validator = validator;
            _storage = storage;

        }
        
        public async Task<(IEnumerable<Topic> resources, int totalCount)> Execute(
            GetTopicsQuery query, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(query, cancellationToken);

            return await _storage.GetTopics(query.ForumId, query.Skip, query.Take, cancellationToken);
        }
    }
}
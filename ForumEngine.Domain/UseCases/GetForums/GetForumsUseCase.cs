using ForumEngine.Storage;
using Microsoft.EntityFrameworkCore;

namespace ForumEngine.Domain.UseCases.GetForums
{
    public class GetForumsUseCase : IGetForumsUseCase
    {
        private readonly ForumDbContext _dbContext;

        public GetForumsUseCase(ForumDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Models.Forum>> Execute(CancellationToken cancellationToken) =>
        
            await _dbContext.Forums.
                Select(x => new Models.Forum
                {
                    Id = x.ForumId,
                    Title = x.Title
                }).ToArrayAsync(cancellationToken);
        
    }
}
using Microsoft.EntityFrameworkCore;
using TFA.Storage;
using Forum = TFA.Domain.Models.Forum;

namespace TFA.Domain.UseCases.GetForums
{
    public class GetForumsUseCase : IGetForumsUseCase
    {
        private readonly ForumDbContext forumDbContext;
        public GetForumsUseCase(ForumDbContext forumDbContext)
        {
            this.forumDbContext = forumDbContext;
        }
        public async Task<IEnumerable<Forum>> Execute(CancellationToken cancellationToken) =>
         await forumDbContext.Forums
            .Select(forum => new Forum
            {
                Id = forum.ForumId,
                Title = forum.Title
            })
            .ToArrayAsync(cancellationToken);
        
    }
}

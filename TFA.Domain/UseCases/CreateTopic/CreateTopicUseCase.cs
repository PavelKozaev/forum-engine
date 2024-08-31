using Topic = TFA.Domain.Models.Topic;
using TFA.Storage;
using Microsoft.EntityFrameworkCore;
using TFA.Domain.Exceptions;

namespace TFA.Domain.UseCases.CreateTopic
{
    public class CreateTopicUseCase : ICreateTopicUseCase
    {
        private readonly ForumDbContext dbContext;
        public CreateTopicUseCase(ForumDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Topic> Execute(Guid forumId, string title, Guid authorId, CancellationToken cancellationToken)
        {
            var forumExists = await dbContext.Forums.AnyAsync(f => f.ForumId == forumId, cancellationToken);

            if (!forumExists)
            {
                throw new ForumNotFoundException(forumId);
            }

            return new Topic();
        }
    }
}

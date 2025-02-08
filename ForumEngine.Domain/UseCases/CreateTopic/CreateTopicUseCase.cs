using ForumEngine.Domain.Exceptions;
using ForumEngine.Storage;
using Microsoft.EntityFrameworkCore;
using System.Net.Security;

namespace ForumEngine.Domain.UseCases.CreateTopic
{
    public class CreateTopicUseCase : ICreateTopicUseCase
    {
        private readonly ForumDbContext _dbContext;
        private readonly IGuidFactory _guidFactory;
        private readonly IMomentProvider _momentProvider;

        public CreateTopicUseCase(
            ForumDbContext dbContext, 
            IGuidFactory guidFactory,
            IMomentProvider momentProvider)
        {
            _dbContext = dbContext;
            _guidFactory = guidFactory;
            _momentProvider = momentProvider;
        }

        public async Task<Models.Topic> Execute(Guid forumId, string title, Guid authorId, CancellationToken cancellationToken)
        {
            var forumExists = await _dbContext.Forums.AnyAsync(x => x.ForumId == forumId, cancellationToken);

            if (!forumExists)
                throw new ForumNotFoundException(forumId);

            var topicId = _guidFactory.Create();

            await _dbContext.Topics.AddAsync(new Topic
            {
                TopicId = topicId,
                ForumId = forumId,
                UserId = authorId,
                CreatedAt = _momentProvider.Now,
                Title = title
            }, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return await _dbContext.Topics
                .Where(x => x.TopicId == topicId)
                .Select(x => new Models.Topic
                {
                    Id = x.TopicId,
                    Title = x.Title,
                    CreatedAt = x.CreatedAt,
                    Author = x.Author.Login
                }).FirstAsync(cancellationToken);            
        }
    }
}
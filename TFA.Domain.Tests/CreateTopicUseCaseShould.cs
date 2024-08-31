using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Storage;

namespace TFA.Domain.Tests
{
    public class CreateTopicUseCaseShould
    {
        private readonly CreateTopicUseCase sut;
        private readonly ForumDbContext forumDbContext;
        public CreateTopicUseCaseShould()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<ForumDbContext>().UseInMemoryDatabase(nameof(CreateTopicUseCaseShould));
            forumDbContext = new ForumDbContext(dbContextOptionsBuilder.Options);
            sut = new CreateTopicUseCase(forumDbContext);                
        }

        [Fact]
        public async Task ThrowForumNotFoundException_WhenNoMatchingForum()
        {
            await forumDbContext.Forums.AddAsync(new Forum
            {
                ForumId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Title = "Basic Forum",
            });

            await forumDbContext.SaveChangesAsync();

            var forumId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var authorId = Guid.Parse("00000000-0000-0000-0000-000000000011");

            await sut.Invoking(s => s.Execute(forumId, "Some title", authorId, CancellationToken.None))
                .Should().ThrowAsync<ForumNotFoundException>();
        }
    }
}
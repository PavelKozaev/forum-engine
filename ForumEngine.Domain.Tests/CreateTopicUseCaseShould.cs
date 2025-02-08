using FluentAssertions;
using ForumEngine.Domain.Exceptions;
using ForumEngine.Domain.UseCases.CreateTopic;
using ForumEngine.Storage;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Language.Flow;

namespace ForumEngine.Domain.Tests
{
    public class CreateTopicUseCaseShould
    {
        private readonly CreateTopicUseCase sut;
        private readonly ForumDbContext forumDbContext;
        private readonly ISetup<IGuidFactory, Guid> createIdSetup;
        private readonly ISetup<IMomentProvider, DateTimeOffset> getNowSetup;

        public CreateTopicUseCaseShould()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<ForumDbContext>()
                .UseInMemoryDatabase(nameof(CreateTopicUseCaseShould));
            forumDbContext = new ForumDbContext(dbContextOptionsBuilder.Options);

            var guidFactory = new Mock<IGuidFactory>();
            createIdSetup = guidFactory.Setup(x => x.Create());

            var momentProvider = new Mock<IMomentProvider>();
            getNowSetup = momentProvider.Setup(x => x.Now);

            sut = new CreateTopicUseCase(forumDbContext, guidFactory.Object, momentProvider.Object);
        }

        [Fact]
        public async Task ThrowForumNotFoundException_WhetNoMatchingForum()
        {
            await forumDbContext.Forums.AddAsync(new Storage.Forum
            {
                ForumId = Guid.Parse("65b3484c-6c44-4717-8c82-8b95677feb23"),
                Title = "Basic forum"
            });
            await forumDbContext.SaveChangesAsync();

            var forumId = Guid.Parse("53b3477c-6c44-4717-8c82-8b95677feb89");
            var authorId = Guid.Parse("75b3488c-6c44-4717-8c82-9b95677feb43");

            await sut.Invoking(x => x.Execute(forumId, "Some tile", authorId, CancellationToken.None))
                .Should().ThrowAsync<ForumNotFoundException>();
        }

        [Fact]
        public async Task ReturnNewlyCreatedTopic()
        {
            var forumId = Guid.Parse("54b3484c-6c44-4717-8c82-8b95677feb29");
            var userId = Guid.Parse("82b3484c-6c44-4717-8c82-8b95677feb76");

            await forumDbContext.Forums.AddAsync(new Storage.Forum
            {
                ForumId = forumId,
                Title = "Existing forum"
            });
            await forumDbContext.Users.AddAsync(new User
            {
                UserId = userId,
                Login = "Pako"
            });
            await forumDbContext.SaveChangesAsync();

            createIdSetup.Returns(Guid.Parse("87b3484c-6c44-4717-8c82-8b95677feb33"));
            getNowSetup.Returns(new DateTimeOffset(2023, 07, 11, 19, 17, 00, TimeSpan.FromHours(3)));

            var actual = await sut.Execute(forumId, "Hello world", userId, CancellationToken.None);

            var allTopics = await forumDbContext.Topics.ToArrayAsync();
            allTopics.Should().BeEquivalentTo(new[]
            {
                new Storage.Topic
                {
                    ForumId = forumId,
                    UserId = userId,
                    Title = "Hello world"
                }
            }, cfg => cfg.Including(x => x.ForumId).Including(x => x.UserId).Including(x => x.Title));

            actual.Should().BeEquivalentTo(new Models.Topic
            {
                Id = Guid.Parse("87b3484c-6c44-4717-8c82-8b95677feb33"),
                Title = "Hello world",
                Author = "Pako",
                CreatedAt = new DateTimeOffset(2023, 07, 11, 19, 17, 00, TimeSpan.FromHours(3))
            });
        }
    }
}
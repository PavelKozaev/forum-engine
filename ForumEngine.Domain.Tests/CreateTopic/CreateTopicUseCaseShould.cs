using FluentAssertions;
using FluentValidation;
using ForumEngine.Domain.Authentication;
using ForumEngine.Domain.Authorization;
using ForumEngine.Domain.Exceptions;
using ForumEngine.Domain.Models;
using ForumEngine.Domain.UseCases.CreateTopic;
using ForumEngine.Domain.UseCases.GetForums;
using Moq;
using Moq.Language.Flow;

namespace ForumEngine.Domain.Tests.CreateTopic
{
    public class CreateTopicUseCaseShould
    {
        private readonly CreateTopicUseCase sut;
        private readonly Mock<ICreateTopicStorage> storage;
        private readonly ISetup<ICreateTopicStorage, Task<Topic>> createTopicSetup;
        private readonly ISetup<IIdentity, Guid> getCurrentUserIdSetup;
        private readonly ISetup<IIntentionManager, bool> intentionIsAllowedSetup;
        private readonly Mock<IIntentionManager> intentionManger;
        private readonly Mock<IGetForumsStorage> getForumsStorage;
        private readonly ISetup<IGetForumsStorage, Task<IEnumerable<Forum>>> getForumsSetup;

        public CreateTopicUseCaseShould()
        {
            storage = new Mock<ICreateTopicStorage>();
            createTopicSetup = storage
                .Setup(x => x.CreateTopic(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

            getForumsStorage = new Mock<IGetForumsStorage>();
            getForumsSetup = getForumsStorage.Setup(s => s.GetForums(It.IsAny<CancellationToken>()));
            
            var identity = new Mock<IIdentity>();
            var identityProvider = new Mock<IIdentityProvider>();
            identityProvider.Setup(x => x.Current).Returns(identity.Object);
            getCurrentUserIdSetup = identity.Setup(x => x.UserId);

            intentionManger = new Mock<IIntentionManager>();
            intentionIsAllowedSetup = intentionManger.Setup(x => x.IsAllowed(It.IsAny<TopicIntention>()));

            var validator = new Mock<IValidator<CreateTopicCommand>>();
            validator
                .Setup(v => v.ValidateAsync(It.IsAny<CreateTopicCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            sut = new CreateTopicUseCase(validator.Object, intentionManger.Object, identityProvider.Object, getForumsStorage.Object, storage.Object);
        }

        [Fact]
        public async Task ThrowIntentionManagerException_WhenTopicCreationIsNotAllowed()
        {
            var forumId = Guid.Parse("62b3477c-6c44-4717-8c82-8b95677feb49");

            intentionIsAllowedSetup.Returns(false);

            await sut.Invoking(x => x.Execute(new CreateTopicCommand(forumId, "Whatever"), CancellationToken.None))
                .Should().ThrowAsync<IntentionManagerException>();

            intentionManger.Verify(x => x.IsAllowed(TopicIntention.Create));
        }

        [Fact]
        public async Task ThrowForumNotFoundException_WhetNoMatchingForum()
        {
            var forumId = Guid.Parse("53b3477c-6c44-4717-8c82-8b95677feb89");

            intentionIsAllowedSetup.Returns(true);
            getForumsSetup.ReturnsAsync(Array.Empty<Forum>());
                        
            await sut.Invoking(x => x.Execute(new CreateTopicCommand(forumId, "Some tile"), CancellationToken.None))
                .Should().ThrowAsync<ForumNotFoundException>();
        }

        [Fact]
        public async Task ReturnNewlyCreatedTopic_WhenMatchingForumExists()
        {
            var forumId = Guid.Parse("54b3484c-6c44-4717-8c82-8b95677feb29");
            var userId = Guid.Parse("82b3484c-6c44-4717-8c82-8b95677feb76");

            intentionIsAllowedSetup.Returns(true);
            getForumsSetup.ReturnsAsync([new () { Id = forumId }]);
            getCurrentUserIdSetup.Returns(userId);

            var expected = new Models.Topic();
            createTopicSetup.ReturnsAsync(expected);                       

            var actual = await sut.Execute(new CreateTopicCommand(forumId, "Hello world"), CancellationToken.None);
            actual.Should().Be(expected);

            storage.Verify(x => x.CreateTopic(forumId, userId, "Hello world", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
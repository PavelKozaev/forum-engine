using FluentAssertions;
using ForumEngine.Domain.Exceptions;
using ForumEngine.Domain.Authentication;
using ForumEngine.Domain.UseCases.CreateTopic;
using Moq;
using Moq.Language.Flow;
using ForumEngine.Domain.Authorization;
using FluentValidation;

namespace ForumEngine.Domain.Tests
{
    public class CreateTopicUseCaseShould
    {
        private readonly CreateTopicUseCase sut;
        private readonly Mock<ICreateTopicStorage> storage;
        private readonly ISetup<ICreateTopicStorage, Task<bool>> forumExistsSetup;
        private readonly ISetup<ICreateTopicStorage, Task<Models.Topic>> createTopicSetup;
        private readonly ISetup<IIdentity, Guid> getCurrentUserIdSetup;
        private readonly ISetup<IIntentionManager, bool> intentionIsAllowedSetup;
        private readonly Mock<IIntentionManager> intentionManger;

        public CreateTopicUseCaseShould()
        {
            storage = new Mock<ICreateTopicStorage>();
            forumExistsSetup = storage.Setup(x => x.ForumExists(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));
            createTopicSetup = storage.Setup(x => x.CreateTopic(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

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

            sut = new CreateTopicUseCase(validator.Object, intentionManger.Object, identityProvider.Object, storage.Object);
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
            forumExistsSetup.ReturnsAsync(false);
                        
            await sut.Invoking(x => x.Execute(new CreateTopicCommand(forumId, "Some tile"), CancellationToken.None))
                .Should().ThrowAsync<ForumNotFoundException>();

            storage.Verify(x => x.ForumExists(forumId, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task ReturnNewlyCreatedTopic_WhenMatchingForumExists()
        {
            var forumId = Guid.Parse("54b3484c-6c44-4717-8c82-8b95677feb29");
            var userId = Guid.Parse("82b3484c-6c44-4717-8c82-8b95677feb76");

            intentionIsAllowedSetup.Returns(true);
            forumExistsSetup.ReturnsAsync(true);
            getCurrentUserIdSetup.Returns(userId);

            var expected = new Models.Topic();
            createTopicSetup.ReturnsAsync(expected);                       

            var actual = await sut.Execute(new CreateTopicCommand(forumId, "Hello world"), CancellationToken.None);
            actual.Should().Be(expected);

            storage.Verify(x => x.CreateTopic(forumId, userId, "Hello world", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
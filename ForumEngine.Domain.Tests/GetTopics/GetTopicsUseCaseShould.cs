using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ForumEngine.Domain.Exceptions;
using ForumEngine.Domain.Models;
using ForumEngine.Domain.UseCases.GetForums;
using ForumEngine.Domain.UseCases.GetTopics;
using Moq;
using Moq.Language.Flow;

namespace ForumEngine.Domain.Tests.GetTopics
{
    public class GetTopicsUseCaseShould
    {
        private readonly GetTopicsUseCase sut;
        private readonly Mock<IGetTopicsStorage> storage;
        private readonly ISetup<IGetTopicsStorage,Task<(IEnumerable<Topic> resources, int totalCount)>> getTopicsSetup;
        private readonly ISetup<IGetForumsStorage,Task<IEnumerable<Forum>>> getForumsSetup;
        
        public GetTopicsUseCaseShould()
        {
            var validator = new Mock<IValidator<GetTopicsQuery>>();
            validator
                .Setup(v => v.ValidateAsync(It.IsAny<GetTopicsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());
            
            var getForumsStorage = new Mock<IGetForumsStorage>();
            getForumsSetup = getForumsStorage.Setup(s => s.GetForums(It.IsAny<CancellationToken>()));

            storage = new Mock<IGetTopicsStorage>();
            getTopicsSetup = storage.Setup(s =>
                s.GetTopics(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()));
            
            sut = new GetTopicsUseCase(validator.Object, getForumsStorage.Object, storage.Object);
        }
        
        [Fact]
        public async Task ThrowForumNotFoundException_WhenNoForum()
        {
            var forumId = Guid.Parse("64C3B227-8D4A-4A0E-A161-04F19C2ABBC4");

            getForumsSetup.ReturnsAsync(new Forum[] { new() { Id = Guid.Parse("01B1C554-184B-4B32-913E-F7031AAD3BAC") } });

            var query = new GetTopicsQuery(forumId, 0, 1);
            await sut.Invoking(s => s.Execute(query, CancellationToken.None))
                .Should().ThrowAsync<ForumNotFoundException>();
        }
        
        [Fact]
        public async Task ReturnTopics_ExtractedFromStorage_WhenTopicsExists()
        {
            var forumId = Guid.Parse("cf8df9e5-cc85-4e99-b3cb-f2c34c3ceea7");
            
            getForumsSetup.ReturnsAsync(new Forum[] { new() { Id = Guid.Parse("cf8df9e5-cc85-4e99-b3cb-f2c34c3ceea7") } });

            var expectedResources = new Topic[] {new Topic()};
            var expectedTotalCount = 6;
            
            getTopicsSetup.ReturnsAsync((expectedResources, expectedTotalCount));
            
            var (actualResources, actualTotalCount) = await sut.Execute(
                new GetTopicsQuery(forumId, 5, 10), CancellationToken.None);
            
            actualResources.Should().BeEquivalentTo(expectedResources);
            actualTotalCount.Should().Be(expectedTotalCount);
            storage.Verify(s => s.GetTopics(forumId, 5, 10, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
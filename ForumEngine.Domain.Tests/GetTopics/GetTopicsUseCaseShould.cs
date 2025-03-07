using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ForumEngine.Domain.Models;
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
        
        public GetTopicsUseCaseShould()
        {
            var validator = new Mock<IValidator<GetTopicsQuery>>();
            validator
                .Setup(v => v.ValidateAsync(It.IsAny<GetTopicsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            storage = new Mock<IGetTopicsStorage>();
            getTopicsSetup = storage.Setup(s =>
                s.GetTopics(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()));
            
            sut = new GetTopicsUseCase(validator.Object, storage.Object);
        }
        
        [Fact]
        public async Task ReturnTopics_ExtractedFromStorage()
        {
            var forumId = Guid.Parse("cf8df9e5-cc85-4e99-b3cb-f2c34c3ceea7");

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
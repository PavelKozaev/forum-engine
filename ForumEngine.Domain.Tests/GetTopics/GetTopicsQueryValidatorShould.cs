using FluentAssertions;
using ForumEngine.Domain.UseCases.GetTopics;

namespace ForumEngine.Domain.Tests.GetTopics
{
    public class GetTopicsQueryValidatorShould
    {
        private readonly GetTopicsQueryValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenQueryIsValid()
        {
            var query = new GetTopicsQuery(
                Guid.Parse("55357bd9-9bd1-43a5-9f85-8d760dbf3fcc"), 
                10, 
                5);

            sut.Validate(query).IsValid.Should().BeTrue();
        }

        public static IEnumerable<object[]> GetInvalidQuery()
        {
            var query = new GetTopicsQuery(
                Guid.Parse("55357bd9-9bd1-43a5-9f85-8d760dbf3fcc"), 
                10, 
                5);

            yield return [query with { ForumId = Guid.Empty }];
            yield return [query with { Skip = -1 }];
            yield return [query with { Take = -1 }];
        }

        [Theory]
        [MemberData(nameof(GetInvalidQuery))]
        public void ReturnFailure_WhenQueryIsInvalid(GetTopicsQuery query)
        {
            sut.Validate(query).IsValid.Should().BeFalse();
        }
    }
}
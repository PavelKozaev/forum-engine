using FluentAssertions;
using ForumEngine.Domain.UseCases.CreateTopic;

namespace ForumEngine.Domain.Tests.CreateTopic
{
    public class CreateTopicCommandValidatorShould
    {
        private readonly CreateTopicCommandValidator sut = new();

        [Fact]
        public void ReturnSuccess_WhenCommandIsValid()
        {
            var actual = sut.Validate(new CreateTopicCommand(Guid.Parse("41b3484c-6c44-4717-8c82-8b95677feb28"), "Hello"));
            actual.IsValid.Should().BeTrue();
        }

        public static IEnumerable<object[]> GetInvalidCommands()
        {
            var validCommand = new CreateTopicCommand(Guid.Parse("74b3484c-6c44-4717-8c82-8b95677feb14"), "Hello");
            yield return new object[] { validCommand with { ForumId = Guid.Empty } };
            yield return new object[] { validCommand with { Title = string.Empty } };
            yield return new object[] { validCommand with { Title = "          " } };
            yield return new object[] { validCommand with { Title = string.Join("a", Enumerable.Range(0, 100)) } };
        }

        [Theory]
        [MemberData(nameof(GetInvalidCommands))]
        public void ReturnFailure_WhenCommandIsInvalid(CreateTopicCommand command)
        {
            var actual = sut.Validate(command);
            actual.IsValid.Should().BeFalse();
        }
    }
}
using FluentValidation;
using ForumEngine.Domain.Exceptions;

namespace ForumEngine.Domain.UseCases.CreateForum
{
    internal class CreateForumCommandValidator : AbstractValidator<CreateForumCommand>
    {
        public CreateForumCommandValidator()
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithErrorCode(ValidationErrorCode.Empty)
                .MaximumLength(50).WithErrorCode(ValidationErrorCode.TooLong);
        }
    }
}
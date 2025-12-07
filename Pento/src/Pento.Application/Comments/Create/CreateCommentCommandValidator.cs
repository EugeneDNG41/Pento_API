using FluentValidation;

namespace Pento.Application.Comments.Create;

internal sealed class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
    }
}

using FluentValidation;

namespace Pento.Application.BlogPosts.Create;

internal sealed class CreateBlogPostCommandValidator : AbstractValidator<CreateBlogPostCommand>
{
    public CreateBlogPostCommandValidator()
    {

    }
}

using FluentValidation;

namespace Pento.Application.GiveawayPosts.Delete;

public sealed class DeleteGiveawayPostCommandValidator
    : AbstractValidator<DeleteGiveawayPostCommand>
{
    public DeleteGiveawayPostCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("GiveawayPost Id is required.");
    }
}

using FluentValidation;

namespace Pento.Application.GiveawayPosts.Update;

public sealed class UpdateGiveawayPostCommandValidator
    : AbstractValidator<UpdateGiveawayPostCommand>
{
    public UpdateGiveawayPostCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.TitleDescription)
            .MaximumLength(500);

        RuleFor(x => x.ContactInfo)
            .MaximumLength(200);

        RuleFor(x => x.Address)
            .MaximumLength(300);

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .When(x => x.Quantity.HasValue)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid status value.");

        When(x => x.PickupStartDate.HasValue && x.PickupEndDate.HasValue, () =>
        {
            RuleFor(x => x.PickupEndDate)
                .GreaterThanOrEqualTo(x => x.PickupStartDate)
                .WithMessage("PickupEndDate must be after PickupStartDate.");
        });
    }
}

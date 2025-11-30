using FluentValidation;
namespace Pento.Application.GiveawayPosts.Create;
public sealed class CreateGiveawayPostCommandValidator
    : AbstractValidator<CreateGiveawayPostCommand>
{
    public CreateGiveawayPostCommandValidator()
    {
        RuleFor(x => x.FoodItemId)
            .NotEmpty().WithMessage("FoodItemId is required.");

        RuleFor(x => x.TitleDescription)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500);

        RuleFor(x => x.ContactInfo)
            .NotEmpty().WithMessage("Contact information is required.")
            .MaximumLength(200);

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(300);

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.PickupOption)
            .IsInEnum().WithMessage("Invalid pickup option value.");

        When(x => x.PickupStartDate.HasValue, () =>
        {
            RuleFor(x => x.PickupEndDate)
                .NotNull().WithMessage("PickupEndDate must be provided when PickupStartDate is provided.")
                .GreaterThanOrEqualTo(x => x.PickupStartDate)
                .WithMessage("PickupEndDate must be after PickupStartDate.");
        });
    }
}

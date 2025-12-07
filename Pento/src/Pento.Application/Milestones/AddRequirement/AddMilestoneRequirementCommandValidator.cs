using FluentValidation;

namespace Pento.Application.Milestones.AddRequirement;

internal sealed class AddMilestoneRequirementCommandValidator : AbstractValidator<AddMilestoneRequirementCommand>
{
    public AddMilestoneRequirementCommandValidator()
    {
        RuleFor(x => x.MilestoneId)
            .NotEmpty().WithMessage("Milestone Id is required.");
        RuleFor(x => x.ActivityCode)
            .NotEmpty().WithMessage("Activity Code is required.");
        RuleFor(x => x.Quota)
            .GreaterThan(0).WithMessage("Quota must be greater than 0.");
        RuleFor(x => x.WithinDays)
            .GreaterThan(0).When(x => x.WithinDays.HasValue)
            .WithMessage("Within Days must be greater than 0.");
    }
}

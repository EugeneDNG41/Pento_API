using FluentValidation;

namespace Pento.Application.Milestones.Create;

internal sealed class UpdateMilestoneRequirementCommandValidator : AbstractValidator<UpdateMilestoneRequirementCommand>
{
    public UpdateMilestoneRequirementCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Milestone Requirement Id is required.");
        RuleFor(x => x.ActivityCode)
            .NotEmpty().When(x => x.ActivityCode != null)
            .WithMessage("Activity Code must not be empty if provided.");
        RuleFor(x => x.Quota)
            .GreaterThan(0).When(x => x.Quota.HasValue)
            .WithMessage("Quota must be greater than 0.");
        RuleFor(x => x.WithinDays)
            .GreaterThan(0).When(x => x.WithinDays.HasValue)
            .WithMessage("Within Days must be greater than 0.");
    }
}

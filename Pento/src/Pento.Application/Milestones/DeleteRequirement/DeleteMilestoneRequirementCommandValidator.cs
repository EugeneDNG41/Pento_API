using FluentValidation;

namespace Pento.Application.Milestones.DeleteRequirement;

internal sealed class DeleteMilestoneRequirementCommandValidator : AbstractValidator<DeleteMilestoneRequirementCommand>
{
    public DeleteMilestoneRequirementCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Milestone Requirement Id is required.");
    }
}

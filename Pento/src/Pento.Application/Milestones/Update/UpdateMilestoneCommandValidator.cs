using FluentValidation;

namespace Pento.Application.Milestones.Update;

internal sealed class UpdateMilestoneCommandValidator : AbstractValidator<UpdateMilestoneCommand>
{
    public UpdateMilestoneCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Milestone Id is required.");
        RuleFor(x => x.Name)
            .NotEmpty().When(x => x.Name != null)
            .WithMessage("Name must not be empty if provided.")
            .MaximumLength(20).WithMessage("Name must not exceed 20 characters.");
        RuleFor(x => x.Description)
            .NotEmpty().When(x => x.Description != null)
            .WithMessage("Description must not be empty if provided.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}

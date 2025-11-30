using FluentValidation;

namespace Pento.Application.Milestones.Delete;

internal sealed class DeleteMilestoneCommandValidator : AbstractValidator<DeleteMilestoneCommand>
{
    public DeleteMilestoneCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Milestone Id is required.");
    }
}

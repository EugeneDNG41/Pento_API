using FluentValidation;

namespace Pento.Application.Milestones.GetById;

internal sealed class GetMilestoneByIdQueryValidator : AbstractValidator<GetMilestoneByIdQuery>
{
    public GetMilestoneByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Milestone Id is required.");
    }
}

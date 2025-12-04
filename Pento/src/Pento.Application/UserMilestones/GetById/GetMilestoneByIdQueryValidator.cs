using FluentValidation;

namespace Pento.Application.UserMilestones.GetById;

internal sealed class GetMilestoneByIdQueryValidator : AbstractValidator<GetMilestoneByIdQuery>
{
    public GetMilestoneByIdQueryValidator()
    {
        RuleFor(x => x.MilestoneId)
            .NotEmpty().WithMessage("Milestone Id is required.");
    }
}

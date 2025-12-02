using FluentValidation;

namespace Pento.Application.Milestones.GetById;

internal sealed class GetAdminMilestoneByIdQueryValidator : AbstractValidator<GetAdminMilestoneByIdQuery>
{
    public GetAdminMilestoneByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Milestone Id is required.");
    }
}

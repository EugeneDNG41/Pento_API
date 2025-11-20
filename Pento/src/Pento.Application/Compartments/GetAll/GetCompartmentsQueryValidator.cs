using FluentValidation;

namespace Pento.Application.Compartments.GetAll;

internal sealed class GetCompartmentsQueryValidator : AbstractValidator<GetCompartmentsQuery>
{
    public GetCompartmentsQueryValidator()
    {
        RuleFor(x => x.StorageId)
            .NotEmpty().WithMessage("Storage Id must not be empty.");
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
    }
}

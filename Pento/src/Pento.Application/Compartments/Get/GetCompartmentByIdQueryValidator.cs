using FluentValidation;

namespace Pento.Application.Compartments.Get;

internal sealed class GetCompartmentByIdQueryValidator : AbstractValidator<GetCompartmentByIdQuery>
{
    public GetCompartmentByIdQueryValidator()
    {
        RuleFor(x => x.CompartmentId).NotEmpty().WithMessage("Compartment Id is required.");
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
    }
}

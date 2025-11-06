using FluentValidation;

namespace Pento.Application.Compartments.Get;

internal sealed class GetCompartmentByIdQueryValidator : AbstractValidator<GetCompartmentByIdQuery>
{
    public GetCompartmentByIdQueryValidator()
    {
        RuleFor(x => x.CompartmentId).NotEmpty().WithMessage("Compartment Id must not be empty.");
    }
}

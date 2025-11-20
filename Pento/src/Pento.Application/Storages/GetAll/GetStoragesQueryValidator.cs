using FluentValidation;

namespace Pento.Application.Storages.GetAll;

internal sealed class GetStoragesQueryValidator : AbstractValidator<GetStoragesQuery>
{
    public GetStoragesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
    }
}

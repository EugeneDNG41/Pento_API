using FluentValidation;

namespace Pento.Application.Storages.GetById;

internal sealed class GetStorageByIdQueryValidator : AbstractValidator<GetStorageByIdQuery>
{
    public GetStorageByIdQueryValidator()
    {
        RuleFor(x => x.StorageId)
            .NotEmpty().WithMessage("Storage Id must not be empty.");
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.");
    }
}

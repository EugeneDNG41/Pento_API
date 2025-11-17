using FluentValidation;

namespace Pento.Application.FoodReferences.Scan;

internal sealed class ScanBarcodeValidator : AbstractValidator<ScanBarcodeQuery>
{
    public ScanBarcodeValidator()
    {
        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("Barcode must be provided.")
            .Matches("^[0-9]+$").WithMessage("Barcode must contain only numeric characters.");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.StorageItems.Create;

internal sealed class CreateStorageItemCommandValidator : AbstractValidator<CreateStorageItemCommand>
{
    public CreateStorageItemCommandValidator()
    {
        RuleFor(x => x.FoodRefId).NotEmpty();
        RuleFor(x => x.CompartmentId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitId).NotEmpty();
        RuleFor(x => x.ExpirationDateUtc).GreaterThan(DateTime.UtcNow);
    }
}

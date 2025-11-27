using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.FoodReferences.Delete;
internal sealed class DeleteFoodReferenceCommandValidator : AbstractValidator<DeleteFoodReferenceCommand>
{
    public DeleteFoodReferenceCommandValidator()
    {
        RuleFor(x => x.FoodReferenceId)
            .NotEmpty()
            .WithMessage("Food Reference Code is required.")
            .Must(id => id != Guid.Empty)
            .WithMessage("Food Reference Code must be a valid GUID.");
    }
}

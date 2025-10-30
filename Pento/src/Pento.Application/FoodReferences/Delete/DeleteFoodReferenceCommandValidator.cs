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
            .WithMessage("FoodReferenceId must not be empty.")
            .Must(id => id != Guid.Empty)
            .WithMessage("FoodReferenceId must be a valid GUID.");
    }
}

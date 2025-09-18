using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Modules.Pantry.Application.FoodItems.Create;

internal sealed class CreateFoodItemCommandValidator : AbstractValidator<CreateFoodItemCommand>
{
    public CreateFoodItemCommandValidator()
    {     
    }
}

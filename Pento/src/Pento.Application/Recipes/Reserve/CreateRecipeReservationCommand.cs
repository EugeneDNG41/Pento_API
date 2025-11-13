using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Users;

namespace Pento.Application.Recipes.Reserve;
public sealed record CreateRecipeReservationCommand(
    Guid FoodItemId,
    decimal Quantity,
    Guid UnitId,
    Guid RecipeId
) : ICommand<Guid>;

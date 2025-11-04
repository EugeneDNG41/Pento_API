using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.RecipeIngredients.Update;
public sealed record UpdateRecipeIngredientCommand(Guid Id, decimal Quantity,Guid UnitId, string? Notes): ICommand;


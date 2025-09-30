using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Common.Application.Messaging;

namespace Pento.Modules.Pantry.Application.FoodItems.Create;

public sealed record CreateFoodItemCommand( //to be filled
    ) : ICommand<Guid>
{
}

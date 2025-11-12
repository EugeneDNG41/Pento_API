using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryLists.Get;
public sealed record GetGroceryListsByHouseholdIdQuery(Guid HouseholdId)
    : IQuery<IReadOnlyList<GroceryListResponse>>;

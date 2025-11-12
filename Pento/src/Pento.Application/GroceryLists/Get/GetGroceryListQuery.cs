using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryLists.Get;

public sealed record GetGroceryListQuery(Guid Id) : IQuery<GroceryListResponse>;

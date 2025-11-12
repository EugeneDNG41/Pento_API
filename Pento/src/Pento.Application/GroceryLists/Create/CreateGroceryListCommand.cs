using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.GroceryLists.Create;
public sealed record CreateGroceryListCommand(
    string Name
) : ICommand<Guid>;

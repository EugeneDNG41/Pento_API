using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.Create;

public sealed record CreateHouseholdCommand(string Name, Guid UserId) : ICommand<Guid>;

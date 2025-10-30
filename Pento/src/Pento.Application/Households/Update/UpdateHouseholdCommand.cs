using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.Update;

public sealed record UpdateHouseholdCommand(Guid HouseholdId, string Name) : ICommand;

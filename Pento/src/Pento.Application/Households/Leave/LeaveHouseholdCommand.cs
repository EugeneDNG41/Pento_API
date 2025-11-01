using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Households;

namespace Pento.Application.Households.Leave;

public sealed record LeaveHouseholdCommand() : ICommand;

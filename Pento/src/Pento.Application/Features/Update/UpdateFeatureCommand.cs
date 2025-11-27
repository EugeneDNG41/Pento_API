using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Shared;

namespace Pento.Application.Features.Update;

public sealed record UpdateFeatureCommand(
    string Code,
    string? Name,
    string? Description,
    int? DefaultQuota,
    TimeUnit? DefaultResetPeriod
) : ICommand;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Households;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.Update;

public sealed record UpdateStorageCommand(Guid StorageId, Guid? HouseholdId, string Name, StorageType Type, string? Notes) : ICommand;

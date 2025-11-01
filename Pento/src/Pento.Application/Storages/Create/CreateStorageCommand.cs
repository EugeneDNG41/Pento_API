using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.Create;

public sealed record CreateStorageCommand(string Name, StorageType Type, string? Notes) : ICommand<Guid>;

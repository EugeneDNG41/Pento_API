using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Storages.Update;

public sealed record UpdateStorageCommand(Guid StorageId, string Name, string? Notes) : ICommand;

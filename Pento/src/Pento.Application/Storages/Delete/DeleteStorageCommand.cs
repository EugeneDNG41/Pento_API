using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Storages.Delete;

public sealed record DeleteStorageCommand(Guid StorageId) : ICommand;

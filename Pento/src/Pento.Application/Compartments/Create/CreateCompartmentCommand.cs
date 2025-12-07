using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Compartments.Create;

public sealed record CreateCompartmentCommand(Guid StorageId, string Name, string? Notes) : ICommand<Guid>;

using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Compartments.Update;

public sealed record UpdateCompartmentCommand(Guid Id, string Name, string? Notes) : ICommand;

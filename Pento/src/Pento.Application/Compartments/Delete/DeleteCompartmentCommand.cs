using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Compartments.Delete;

public sealed record DeleteCompartmentCommand(Guid CompartmentId) : ICommand;

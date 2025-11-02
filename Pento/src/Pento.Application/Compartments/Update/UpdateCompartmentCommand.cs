using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Households;
using Pento.Domain.Users;

namespace Pento.Application.Compartments.Update;

public sealed record UpdateCompartmentCommand(Guid Id, string Name, string? Notes) : ICommand;

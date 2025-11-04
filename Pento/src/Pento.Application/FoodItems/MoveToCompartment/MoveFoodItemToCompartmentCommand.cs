using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.MoveToCompartment;

public sealed record MoveFoodItemToCompartmentCommand(Guid Id, Guid NewCompartmentId, int Version) : ICommand;

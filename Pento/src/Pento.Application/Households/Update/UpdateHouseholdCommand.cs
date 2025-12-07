using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.Update;

public sealed record UpdateHouseholdCommand(string Name) : ICommand;

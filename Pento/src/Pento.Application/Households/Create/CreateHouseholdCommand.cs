using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.Create;

public sealed record CreateHouseholdCommand(string Name) : ICommand<string>;

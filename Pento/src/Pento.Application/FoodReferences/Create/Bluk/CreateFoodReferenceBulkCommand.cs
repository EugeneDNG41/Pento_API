using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodReferences.Create.Bluk;

public sealed record CreateFoodReferenceBulkCommand(
    List<CreateFoodReferenceCommand> Commands
) : ICommand;

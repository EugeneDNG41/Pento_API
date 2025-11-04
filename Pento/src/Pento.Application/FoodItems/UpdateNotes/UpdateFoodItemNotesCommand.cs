using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.UpdateNotes;

public sealed record UpdateFoodItemNotesCommand(Guid Id, string NewNotes, int Version) : ICommand;

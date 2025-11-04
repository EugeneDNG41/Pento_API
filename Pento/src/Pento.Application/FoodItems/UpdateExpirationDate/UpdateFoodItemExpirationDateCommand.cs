using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.UpdateExpirationDate;

public sealed record UpdateFoodItemExpirationDateCommand(Guid Id, DateTime NewExpirationDate, int Version) : ICommand;

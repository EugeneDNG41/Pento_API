using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItemLogs.GetById;

public sealed record GetFoodItemLogByIdQuery(Guid Id) : IQuery<FoodItemLogDetail>;

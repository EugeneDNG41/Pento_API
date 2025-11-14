using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Units;

namespace Pento.Application.FoodItemLogs.GetById;

public sealed record  GetFoodItemLogByIdQuery(Guid Id) : IQuery<FoodItemLogDetail>;

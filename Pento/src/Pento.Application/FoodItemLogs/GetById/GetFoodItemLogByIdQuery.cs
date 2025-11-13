using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.FoodItemLogs;
using Pento.Domain.Units;

namespace Pento.Application.FoodItemLogs.GetById;

public sealed record  GetFoodItemLogByIdQuery(Guid Id) : IQuery<FoodItemDetail>;

public sealed record FoodItemLogDetailRow
{
    public Guid Id { get; init; }
    public Guid FoodItemId { get; init; }
    public Guid UserId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public Uri? ImageUrl { get; init; }
    public DateTime Timestamp { get; init; }
    public FoodItemLogAction Action { get; init; }
    public decimal BaseQuantity { get; init; }
    public UnitType BaseUnitType { get; init; }
}
public sealed record FoodItemDetail(Guid Id);

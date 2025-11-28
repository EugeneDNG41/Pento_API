using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.GiveawayPosts.Get;
public sealed record GiveawayPostDetailRow
{
    public Guid Id { get; init; }
    public Guid FoodItemId { get; init; }
    public Guid FoodRefId { get; init; }
    public string FoodName { get; init; } = "";
    public Uri? FoodImageUrl { get; init; }
    public string TitleDescription { get; init; } = "";
    public string ContactInfo { get; init; } = "";
    public string Address { get; init; } = "";
    public decimal Quantity { get; init; }
    public string Status { get; init; } = "";
    public DateTime? PickupStartDate { get; init; }
    public DateTime? PickupEndDate { get; init; }
    public string PickupOption { get; init; } = "";
    public DateTime CreatedOnUtc { get; init; }
    public DateTime UpdatedOnUtc { get; init; }
}


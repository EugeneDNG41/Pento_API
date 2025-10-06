using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.GiveawayPosts;
public sealed class GiveawayPost : Entity
{
    public GiveawayPost(
      Guid id,
      Guid userId,
      Guid storageItemId,
      string titleDescription,
      string contactInfo,
      GiveawayStatus status,
      DateTime? pickupStartDate,
      DateTime? pickupEndDate,
      PickupOption pickupOption,
      DateTime createdOnUtc,
      string address,
      decimal quantity)
      : base(id)
    {
        UserId = userId;
        StorageItemId = storageItemId;
        TitleDescription = titleDescription;
        ContactInfo = contactInfo;
        Status = status;
        PickupStartDate = pickupStartDate;
        PickupEndDate = pickupEndDate;
        PickupOption = pickupOption;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
        Address = address;
        Quantity = quantity;
    }

    private GiveawayPost() { }

    public Guid UserId { get; private set; }

    public Guid StorageItemId { get; private set; }

    public string TitleDescription { get; private set; } = string.Empty;

    public string ContactInfo { get; private set; } = string.Empty;

    public GiveawayStatus Status { get; private set; }

    public DateTime? PickupStartDate { get; private set; }

    public DateTime? PickupEndDate { get; private set; }

    public PickupOption PickupOption { get; private set; }

    public string Address { get; private set; } = string.Empty;

    public decimal Quantity { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime UpdatedOnUtc
    {
        get; private set;
    }
}

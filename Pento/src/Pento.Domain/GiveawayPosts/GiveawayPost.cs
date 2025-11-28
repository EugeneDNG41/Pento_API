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
      Guid foodItemId,
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
        FoodItemId = foodItemId;
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

    public Guid FoodItemId { get; private set; }

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
    public void UpdateTitle(string newTitle)
    {
        TitleDescription = newTitle;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public void UpdateContact(string newContact)
    {
        ContactInfo = newContact;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public void UpdateAddress(string newAddress)
    {
        Address = newAddress;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public void UpdatePickupOption(PickupOption option)
    {
        PickupOption = option;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public void UpdatePickupStart(DateTime? date)
    {
        PickupStartDate = date;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public void UpdatePickupEnd(DateTime? date)
    {
        PickupEndDate = date;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public void UpdateQuantity(decimal qty)
    {
        Quantity = qty;
        UpdatedOnUtc = DateTime.UtcNow;
    }

    public void UpdateStatus(GiveawayStatus status, DateTime utcNow)
    {
        Status = status;
        UpdatedOnUtc = utcNow;
    }


}

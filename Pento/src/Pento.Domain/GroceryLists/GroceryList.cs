using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.GroceryLists;
public sealed class GroceryList : Entity
{
    public GroceryList(
       Guid id,
       Guid householdId,
       string name,
       Guid createdBy,
       DateTime createdOnUtc
   ) : base(id)
    {
        HouseholdId = householdId;
        Name = name;
        CreatedBy = createdBy;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }

    private GroceryList() { }

    public Guid HouseholdId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime UpdatedOnUtc { get; private set; }

    public void UpdateName(string name, DateTime updatedOnUtc)
    {
        Name = name;
        UpdatedOnUtc = updatedOnUtc;
    }
    public static GroceryList Create(
       Guid householdId,
       string name,
       Guid createdBy,
       DateTime createdOnUtc,
       Guid userId)
    {
        var groceryList = new GroceryList
        {
            Id = Guid.CreateVersion7(),
            HouseholdId = householdId,
            Name = name,
            CreatedBy = createdBy,
            CreatedOnUtc = createdOnUtc,
            UpdatedOnUtc = createdOnUtc
        };
        groceryList.Raise(new GroceryListCreatedDomainEvent(groceryList.Id, userId));
        return groceryList;
    }
}
public sealed class GroceryListCreatedDomainEvent(Guid groceryListId, Guid userId) : DomainEvent
{
    public Guid GroceryListId { get; } = groceryListId;
    public Guid UserId { get; } = userId;
}

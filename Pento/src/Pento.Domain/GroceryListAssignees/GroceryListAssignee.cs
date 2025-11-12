using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.GroceryListAssignees;
public sealed class GroceryListAssignee : Entity
{
    public Guid GroceryListId { get; private set; }
    public Guid HouseholdMemberId { get; private set; }
    public bool IsCompleted { get; private set; } 
    public DateTime AssignedOnUtc { get; private set; }

    private GroceryListAssignee() { }

    public GroceryListAssignee(Guid id, Guid groceryListId, Guid householdMemberId, DateTime assignedOnUtc)
        : base(id)
    {
        GroceryListId = groceryListId;
        HouseholdMemberId = householdMemberId;
        AssignedOnUtc = assignedOnUtc;
    }

}

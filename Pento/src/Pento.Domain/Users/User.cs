using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Roles;

namespace Pento.Domain.Users;

public sealed class User : Entity
{
    private User()
    {
    }

    public Guid? HouseholdId { get; private set; }
    public Uri? AvatarUrl { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string IdentityId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<Role> Roles => _roles.ToList();

    private readonly List<Role> _roles = [];

    public static User Create(string email, string firstName, string lastName, string identityId, DateTime createdAt)
    {
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            IdentityId = identityId,
            CreatedAt = createdAt
        };

        user.Raise(new UserRegisteredDomainEvent(user.IdentityId));

        return user;
    }
    public void Update(string firstName, string lastName)
    {
        if (FirstName == firstName && LastName == lastName)
        {
            return;
        }

        FirstName = firstName;
        LastName = lastName;
    }
    public void SetHouseholdId(Guid? householdId)
    {
        HouseholdId = householdId;
    }

    public void SetAvatarUrl(Uri? avatarUrl)
    {
        AvatarUrl = avatarUrl;
    }
    public void SetRoles(IEnumerable<Role> setRoles)
    {
        var toAdd = setRoles.Where(sr => !_roles.Any(r => r.Name == sr.Name)).ToList();
        var toRemove = _roles.Where(r => !setRoles.Any(sr => sr.Name == r.Name)).ToList();
        foreach (Role? role in toAdd)
        {
            _roles.Add(role);
        }
        foreach (Role? role in toRemove)
        {
            _roles.Remove(role);
        }
    }
}

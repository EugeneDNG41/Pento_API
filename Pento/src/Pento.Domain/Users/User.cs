using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Users;

public sealed class User : Entity
{
    private readonly List<Role> _roles = [];

    private User()
    {
    }

    public Guid HouseholdId { get; private set; }
    public Uri? AvatarUrl { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string IdentityId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<Role> Roles => _roles.ToList();

    public static User Create(string email, string firstName, string lastName, string identityId, DateTime createdAt)
    {
        var Id = Guid.CreateVersion7();
        var user = new User
        {
            Id = Id,
            HouseholdId = Id,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            IdentityId = identityId,
            CreatedAt = createdAt
        };

        user._roles.Add(Role.BasicMember);

        user.Raise(new UserRegisteredDomainEvent(user.Id));

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
}

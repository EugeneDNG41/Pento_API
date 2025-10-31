using Microsoft.EntityFrameworkCore;
using Pento.Application.Abstractions.Data;
using Pento.Domain.Roles;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Repositories;

internal sealed class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public void Insert(User user)
    {
        foreach (Role role in user.Roles)
        {
            dbContext.Attach(role);
        }

        dbContext.Add(user);
    }
    public void Update(User user)
    {
        foreach (Role role in user.Roles)
        {
            dbContext.Attach(role);
        }

        dbContext.Update(user);
    }
}

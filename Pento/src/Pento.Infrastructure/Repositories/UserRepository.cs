using Microsoft.EntityFrameworkCore;
using Pento.Domain.Users;

namespace Pento.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
    public async Task<User?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<User>().SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public void Insert(User user)
    {
        foreach (Role role in user.Roles)
        {
            DbContext.Attach(role);
        }

        DbContext.Add(user);
    }
}

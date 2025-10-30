using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.Users;

public interface IUserRepository
{
    void Insert(User user);
    void Update(User user);
}

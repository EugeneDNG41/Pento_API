using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.Abstractions;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime Timestamp { get; }
}

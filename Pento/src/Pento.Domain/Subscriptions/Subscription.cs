using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Subscriptions;

public sealed class Subscription : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
}

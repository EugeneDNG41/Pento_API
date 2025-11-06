using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Perks;

public sealed class Perk : Entity // can be enum
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Perk(string name, string description)
    {
        Name = name;
        Description = description;
    }
    public void UpdateDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

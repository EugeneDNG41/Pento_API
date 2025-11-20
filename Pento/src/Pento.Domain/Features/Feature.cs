using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Features;

public sealed class Feature : Entity // can be enum
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Feature(string name, string description)
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
public enum FeatureType
{
    NonConsumable, //change history, detailed filter, data export
    Consumable, //AI suggestions, number of storages/compartments/saved recipes/meal plans/grocery lists and list items
}

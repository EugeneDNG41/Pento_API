using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.FoodReferences;

public sealed class FoodReference : Entity
{
    public string Name { get; private set; }
    public FoodGroup FoodGroup { get; private set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Compartments;

public sealed class Compartment : Entity
{
    public Compartment(Guid id, string name, Guid storageId, string? notes) : base(id)
    {
        Name = name;
        StorageId = storageId;
        Notes = notes;
    }
    private Compartment() { }
    public string Name { get; private set; }
    public Guid StorageId { get; private set; }
    public string? Notes { get; private set; }
}

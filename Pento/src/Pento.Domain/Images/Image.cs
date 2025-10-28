using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Images;

public sealed class Image : Entity
{
    public string Url { get; private set; }
    public ImageType Type { get; private set; }
    public Guid TypeId { get; private set; }

}
public enum ImageType
{
    StoragItem,
    Recipe,
    GiveawayPost,
    Other
}

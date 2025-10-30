using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pento.Domain.Abstractions;

namespace Pento.Domain.Images;

public sealed class Media : Entity
{
    public Media(Uri url, string mimeType, string? caption)
    {
        Url = url;
        MimeType = mimeType;
        Caption = caption;
    }

    private Media()
    {
    }
    public Uri Url { get; private set; }
    public string MimeType { get; private set; }
    public string? Caption { get; private set; }
}

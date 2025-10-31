﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pento.Domain.Abstractions;

namespace Pento.Domain.RecipeMedia;

public sealed class RecipeMedia : Entity
{
    public RecipeMedia(Uri url, Guid recipeId, string mimeType)
    {
        Url = url;
        RecipeId = recipeId;
        MimeType = mimeType;
    }

    private RecipeMedia()
    {
    }
    public Uri Url { get; private set; }
    public Guid RecipeId { get; private set; }
    public string MimeType { get; private set; }
    
}

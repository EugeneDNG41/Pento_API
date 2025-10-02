using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;
using Pento.Domain.Users;

namespace Pento.Domain.BlogPosts;

public sealed class BlogPost : Entity
{
    public BlogPost(
           Guid id,
           Guid userId,
           string title,
           Content content,
           BlogPostType postType,
           DateTime createdOnUtc,
           bool isActive = true,
           bool isModerated = false)
           : base(id)
    {
        UserId = userId;
        Title = title;
        Content = content;
        PostType = postType;
        IsActive = isActive;
        IsModerated = isModerated;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }
    private BlogPost()
    {
    }
    public Guid UserId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public Content Content { get; private set; }

    public BlogPostType PostType { get; private set; }

    public bool IsActive { get; private set; }

    public bool IsModerated { get; private set; }

    public DateTime? ModeratedOnUtc { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime UpdatedOnUtc { get; private set; }

}

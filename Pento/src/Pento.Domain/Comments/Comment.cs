using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.Comments;
public class Comment:Entity
{
    public Comment(
       Guid id,
       Guid blogPostId,
       Guid userId,
       string content,
       DateTime createdOnUtc,
       bool isActive = true,
       bool isModerated = false)
       : base(id)
    {
        BlogPostId = blogPostId;
        UserId = userId;
        Content = content;
        IsActive = isActive;
        IsModerated = isModerated;
        CreatedOnUtc = createdOnUtc;
        UpdatedOnUtc = createdOnUtc;
    }

    private Comment()
    {
    }

    public Guid BlogPostId { get; private set; }

    public Guid UserId { get; private set; }

    public string Content { get; private set; } 

    public bool IsActive { get; private set; }

    public bool IsModerated { get; private set; }

    public DateTime? ModeratedOnUtc { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime UpdatedOnUtc { get; private set; }

}

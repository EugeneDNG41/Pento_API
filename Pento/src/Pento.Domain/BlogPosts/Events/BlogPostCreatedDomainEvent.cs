using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.BlogPosts.Events;
public sealed class BlogPostCreatedDomainEvent : DomainEvent
{
    public Guid BlogPostId { get; }
    public Guid UserId { get; }
    public BlogPostCreatedDomainEvent(Guid blogPostId, Guid userId)
    : base()
    {
        BlogPostId = blogPostId;
        UserId = userId;
    }
}


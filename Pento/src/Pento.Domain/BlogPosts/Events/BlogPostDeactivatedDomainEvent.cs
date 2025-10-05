using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Domain.BlogPosts.Events;
public sealed class BlogPostDeactivatedDomainEvent : DomainEvent
{
    public Guid BlogPostId { get; }

    public BlogPostDeactivatedDomainEvent(Guid blogPostId)
        : base()
    {
        BlogPostId = blogPostId;
    }
}

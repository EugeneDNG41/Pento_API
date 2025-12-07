using Pento.Domain.Abstractions;
using Pento.Domain.BlogPosts.Events;
using Pento.Domain.Shared;

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
           bool isActive = true)
           : base(id)
    {
        UserId = userId;
        Title = title;
        Content = content;
        PostType = postType;
        IsActive = isActive;
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


    public DateTime CreatedOnUtc { get; private set; }

    public DateTime UpdatedOnUtc { get; private set; }
    public static BlogPost Create(
     Guid userId,
     string title,
     Content content,
     BlogPostType postType,
     DateTime utcNow)
    {
        var blogPost = new BlogPost(
            Guid.NewGuid(),
            userId,
            title,
            content,
            postType,
            utcNow,
            true
        );

        blogPost.Raise(new BlogPostCreatedDomainEvent(blogPost.Id, blogPost.UserId));
        return blogPost;
    }
}

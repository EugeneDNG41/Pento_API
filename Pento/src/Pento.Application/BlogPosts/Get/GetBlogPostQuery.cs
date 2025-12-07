using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.BlogPosts.Get;

public sealed record GetBlogPostQuery(Guid PostId) : IQuery<BlogPostResponse>;

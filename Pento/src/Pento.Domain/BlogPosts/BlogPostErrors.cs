using Pento.Domain.Abstractions;

namespace Pento.Domain.BlogPosts;

public static class BlogPostErrors
{
    public static readonly Error InvalidPostType = Error.Problem(
      "BlogPost.InvalidPostType",
      "The blog post type provided is invalid."
  );
}

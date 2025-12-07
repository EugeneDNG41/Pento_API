using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.BlogPosts.Create;

public sealed record CreateBlogPostCommand(
) : ICommand<Guid>;

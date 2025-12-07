using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Comments.Get;

public sealed record GetCommentQuery(Guid CommentId) : IQuery<CommentResponse>;

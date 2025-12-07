using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Comments.Create;

public sealed record CreateCommentCommand() : ICommand<Guid>;


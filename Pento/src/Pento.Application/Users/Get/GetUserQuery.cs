

using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.Get;

public sealed record GetUserQuery() : IQuery<UserResponse>;

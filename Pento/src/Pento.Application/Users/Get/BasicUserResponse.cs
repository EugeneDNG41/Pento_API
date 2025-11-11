namespace Pento.Application.Users.Get;

public sealed record BasicUserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    Uri? AvatarUrl
);

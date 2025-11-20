namespace Pento.Application.Users.Search;

public sealed record BasicUserResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    Uri? AvatarUrl
);

public sealed record UserPreview(Guid UserId,
    string? HouseholdName,
    string Email,
    string FirstName,
    string LastName,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsDeleted
);



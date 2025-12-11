namespace Pento.Application.Users.GetAll;

public sealed record BasicUserResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    Uri? AvatarUrl
);


public sealed record UserPreview
{
    public Guid UserId { get; init; }
    public string? HouseholdName { get; init; }
    public string Email { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }
}



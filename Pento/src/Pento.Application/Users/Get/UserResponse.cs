namespace Pento.Application.Users.Get;

public sealed record UserResponse(
    Guid Id,
    Guid? HouseholdId,
    Uri? AvatarUrl,
    string Email, 
    string FirstName, 
    string LastName,
    DateTime CreatedAt);

namespace Pento.Application.Users.Get;

public sealed record UserResponse(Guid Id, string Email, string FirstName, string LastName);

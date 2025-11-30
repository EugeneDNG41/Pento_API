namespace Pento.Application.Abstractions.ThirdPartyServices.Identity;

public sealed record UserModel(string Email, string Password, string FirstName, string LastName);

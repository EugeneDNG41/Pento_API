namespace Pento.Application.Abstractions.Authentication;

public interface IUserContext
{
    Guid UserId { get; }
    Guid? HouseholdId { get; }
    string IdentityId { get; }
    bool IsDeleted { get; }
}

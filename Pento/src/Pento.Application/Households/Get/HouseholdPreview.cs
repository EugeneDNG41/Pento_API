namespace Pento.Application.Households.Get;

public sealed class HouseholdPreview
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string? InviteCode { get; init; }
    public int Members { get; init; }
    public bool IsDeleted { get; init; }
}

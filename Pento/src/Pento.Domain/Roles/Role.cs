namespace Pento.Domain.Roles;

public sealed class Role
{
    public static readonly Role Administrator = new("Administrator", RoleType.Administrative);

    public static readonly Role HouseholdHead = new("Household Head", RoleType.Household);
    public static readonly Role HouseholdMember = new("Household Member", RoleType.Household);



    private Role(string name, RoleType type)
    {
        Name = name;
        Type = type;
    }

    private Role()
    {
    }

    public string Name { get; private set; }
    public RoleType Type { get; private set; }
}
public enum RoleType
{
    Administrative,
    Household
}

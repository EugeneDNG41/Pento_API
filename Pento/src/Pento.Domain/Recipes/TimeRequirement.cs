namespace Pento.Domain.Recipes;

public sealed record TimeRequirement
{
    private TimeRequirement() { }

    public int PrepTimeMinutes { get; init; }

    public int CookTimeMinutes { get; init; }

    public int TotalMinutes => PrepTimeMinutes + CookTimeMinutes;

    public static TimeRequirement Create(int prepTimeMinutes, int cookTimeMinutes)
    {
        if (prepTimeMinutes < 0)
        {
            throw new ArgumentException("Preparation time cannot be negative");
        }

        if (cookTimeMinutes < 0)
        {
            throw new ArgumentException("Cooking time cannot be negative");
        }

        return new TimeRequirement
        {
            PrepTimeMinutes = prepTimeMinutes,
            CookTimeMinutes = cookTimeMinutes
        };
    }
}

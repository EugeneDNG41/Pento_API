namespace Pento.Domain.DietaryTags;

public static class DietaryTagData
{
    public static readonly DietaryTag ContainsAddedSugar = DietaryTag.Create(
        name: "Contains Added Sugar",
        description: "This item contains added sugars beyond natural sources.");

    public static readonly DietaryTag ContainsGluten = DietaryTag.Create(
        name: "Contains Gluten",
        description: "Contains gluten from wheat, barley, rye, or related grains.");

    public static readonly DietaryTag ContainsEgg = DietaryTag.Create(
        name: "Contains Egg",
        description: "Contains egg-based ingredients.");

    public static readonly DietaryTag ContainsPeanuts = DietaryTag.Create(
        name: "Contains Peanuts",
        description: "Contains peanuts or peanut-derived products.");

    public static readonly DietaryTag ContainsTreeNuts = DietaryTag.Create(
        name: "Contains Tree Nuts",
        description: "Contains tree nuts such as almonds, cashews, pistachios, or walnuts.");

    public static readonly DietaryTag ContainsSoy = DietaryTag.Create(
        name: "Contains Soy",
        description: "Contains soy or soy-derived ingredients.");

    public static readonly DietaryTag ContainsAlcohol = DietaryTag.Create(
        name: "Contains Alcohol",
        description: "Contains alcohol or alcohol-based ingredients.");

    public static readonly DietaryTag ContainsCaffeine = DietaryTag.Create(
        name: "Contains Caffeine",
        description: "Contains caffeine from coffee, tea, or other sources.");

    public static readonly DietaryTag HighlyProcessed = DietaryTag.Create(
        name: "Highly Processed",
        description: "This item is highly processed and may contain additives or preservatives.");
}

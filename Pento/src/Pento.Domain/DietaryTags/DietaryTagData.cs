namespace Pento.Domain.DietaryTags;

public static class DietaryTagData
{
    // BASIC NUTRITION TAGS
    public static readonly DietaryTag ContainsSugar = DietaryTag.Create(
        name: "Contains Sugar",
        description: "This item contains added or naturally occurring sugar.");

    public static readonly DietaryTag ContainsFat = DietaryTag.Create(
        name: "Contains Fat",
        description: "This item contains fats from any source.");

    public static readonly DietaryTag ContainsOil = DietaryTag.Create(
        name: "Contains Oil",
        description: "This item contains added oils or oily substances.");

    // COMMON ALLERGENS (Simplified)
    public static readonly DietaryTag ContainsDairy = DietaryTag.Create(
        name: "Contains Dairy",
        description: "This item contains milk or dairy-derived ingredients.");

    public static readonly DietaryTag ContainsEgg = DietaryTag.Create(
        name: "Contains Egg",
        description: "This item contains egg-based ingredients.");

    public static readonly DietaryTag ContainsGluten = DietaryTag.Create(
        name: "Contains Gluten",
        description: "This item contains gluten from wheat, barley, rye, or related grains.");

    public static readonly DietaryTag ContainsTreeNuts = DietaryTag.Create(
        name: "Contains Tree Nuts",
        description: "This item contains tree nuts such as almonds, cashews, pistachios, or walnuts.");

    public static readonly DietaryTag ContainsPeanuts = DietaryTag.Create(
        name: "Contains Peanuts",
        description: "This item contains peanuts or peanut-derived products.");

    public static readonly DietaryTag ContainsSoy = DietaryTag.Create(
        name: "Contains Soy",
        description: "This item contains soy or soy-derived ingredients.");

    public static readonly DietaryTag ContainsSeafood = DietaryTag.Create(
        name: "Contains Seafood",
        description: "This item contains fish, crustaceans, or mollusks.");
}




namespace Pento.Domain.FoodReferences;

public enum FoodGroup
{
    Meat = 1,
    Seafood = 2,
    FruitsVegetables = 3,
    Dairy = 4,
    CerealGrainsPasta = 5,
    LegumesNutsSeeds = 6,
    FatsOils = 7,
    Confectionery = 8,
    Beverages = 9,
    Condiments = 10,
    MixedDishes = 11,
}
public static class FoodGroupExtensions
{
    private static readonly Dictionary<FoodGroup, string> _readable =
        new Dictionary<FoodGroup, string>
        {
            [FoodGroup.Meat] = "Meat",
            [FoodGroup.Seafood] = "Seafood",
            [FoodGroup.FruitsVegetables] = "Fruits & Vegetables",
            [FoodGroup.Dairy] = "Dairy",
            [FoodGroup.CerealGrainsPasta] = "Cereal, Grains & Pasta",
            [FoodGroup.LegumesNutsSeeds] = "Legumes, Nuts & Seeds",
            [FoodGroup.FatsOils] = "Fats & Oils",
            [FoodGroup.Confectionery] = "Confectionery",
            [FoodGroup.Beverages] = "Beverages",
            [FoodGroup.Condiments] = "Condiments",
            [FoodGroup.MixedDishes] = "Mixed Dishes"
        };

    public static string ToReadableString(this FoodGroup value) =>
        _readable.TryGetValue(value, out string? s) ? s : value.ToString();
}

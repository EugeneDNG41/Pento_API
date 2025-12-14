using Pento.Domain.DietaryTags;
using Pento.Domain.FoodDietaryTags;
using Pento.Domain.FoodReferences;

namespace Pento.Infrastructure.Persistence.Seed;

public static class FoodDietaryTagSeeder
{
    public static void Seed(ApplicationDbContext db)
    {
        
        var dietaryTags = db.Set<DietaryTag>().ToList();
        var tagsByName = dietaryTags.ToDictionary(
            x => x.Name,
            x => x,
            StringComparer.OrdinalIgnoreCase);

        var foods = db.Set<FoodReference>().ToList();

        var existing = db.Set<FoodDietaryTag>()
            .Select(x => new { x.FoodReferenceId, x.DietaryTagId })
            .AsEnumerable()
            .Select(x => Tuple.Create(x.FoodReferenceId, x.DietaryTagId))
            .ToHashSet();

        var newLinks = new List<FoodDietaryTag>();

        foreach (FoodReference? food in foods)
        {
            HashSet<Guid> tagIds = GetTagIdsForFood(food, tagsByName);

            foreach (Guid tagId in tagIds)
            {
                var key = Tuple.Create(food.Id, tagId);
                if (!existing.Contains(key))
                {
                    newLinks.Add(FoodDietaryTag.Create(food.Id, tagId));
                    existing.Add(key);
                }
            }
        }

        if (newLinks.Count > 0)
        {
            db.Set<FoodDietaryTag>().AddRange(newLinks);
            db.SaveChanges();
        }
    }

    private static HashSet<Guid> GetTagIdsForFood(
        FoodReference food,
        Dictionary<string, DietaryTag> tagsByName)
    {
        var result = new HashSet<Guid>();

        string name = (food.Name ?? string.Empty).ToLowerInvariant();
        string group = food.FoodGroup.ToString() ?? string.Empty;

        bool isDairyGroup = group.Equals("Dairy", StringComparison.OrdinalIgnoreCase);
        bool isFatsOilsGroup = group.Equals("FatsOils", StringComparison.OrdinalIgnoreCase);
        bool isSeafoodGroup = group.Equals("Seafood", StringComparison.OrdinalIgnoreCase);

        void AddTag(string tagName)
        {
            if (tagsByName.TryGetValue(tagName, out DietaryTag? tag))
            {
                result.Add(tag.Id);
            }
        }

        bool Contains(string token) => name.Contains(token, StringComparison.OrdinalIgnoreCase);
        bool ContainsAny(params string[] tokens) =>
            tokens.Any(t => name.Contains(t, StringComparison.OrdinalIgnoreCase));

        // 1. GLUTEN 
        if (ContainsAny("barley", "rye", "spelt", "semolina", "farro", "kamut", "bulgur", "einkorn"))
        {
            AddTag("Contains Gluten");
        }

        if (Contains("wheat") && !Contains("buckwheat"))
        {
            AddTag("Contains Gluten");
        }

        // Oats 
        if (ContainsAny("oat", "oats"))
        {
            AddTag("Contains Gluten");
        }

        // 2. TREE NUTS
        if (ContainsAny("almond", "cashew", "hazelnut", "pecan", "pistachio", "walnut", "macadamia", "brazil nut"))
        {
            AddTag("Contains Tree Nuts");
        }

        // 3. PEANUTS 
        if (Contains("peanut"))
        {
            AddTag("Contains Peanuts");
        }

        // 4. EGG
        if (Contains("egg"))
        {
            AddTag("Contains Egg");
        }

        // 5. DAIRY 
        bool hasDairyKeywords =
            ContainsAny("milk", "cheese", "yogurt", "cream", "butter", "whey", "casein");

        bool isPlantMilk = ContainsAny("almond", "soy", "oat", "rice", "coconut", "cashew");

        if ((isDairyGroup || hasDairyKeywords) && !Contains("egg") && !isPlantMilk)
        {
            AddTag("Contains Dairy");
        }

        // 6. SEAFOOD 
        if (isSeafoodGroup ||
            ContainsAny(
                "salmon", "cod", "tuna", "tilapia", "pollock", "catfish", "haddock", "hake",
                "snapper", "grouper", "anchovy", "sardine", "trout", "mackerel",
                "shrimp", "prawn", "crab", "lobster", "crayfish", "krill",
                "oyster", "clam", "mussel", "scallop", "squid", "octopus", "snail"
            ))
        {
            AddTag("Contains Seafood");
        }

        // 7. SOY
        if (ContainsAny("soy", "tofu", "edamame", "miso", "tempeh"))
        {
            AddTag("Contains Soy");
        }

        // ===========================
        if (ContainsAny("sugar", "syrup", "honey", "molasses", "caramel", "fructose"))
        {
            AddTag("Contains Sugar");
        }

        // 9. OIL
        if (isFatsOilsGroup || Contains("oil"))
        {
            AddTag("Contains Oil");
        }

        // 10. FAT
        if (isFatsOilsGroup || ContainsAny("butter", "cream", "lard", "tallow", "shortening", "margarine"))
        {
            AddTag("Contains Fat");
        }

        return result;
    }
}

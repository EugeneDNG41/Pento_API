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

        foreach (FoodReference food in foods)
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

        void AddTag(string tagName)
        {
            if (tagsByName.TryGetValue(tagName, out DietaryTag? tag))
            {
                result.Add(tag.Id);
            }
        }

        bool Contains(string token) =>
            name.Contains(token, StringComparison.OrdinalIgnoreCase);

        bool ContainsAny(params string[] tokens) =>
            tokens.Any(t => name.Contains(t, StringComparison.OrdinalIgnoreCase));

        if (ContainsAny(
                "wheat", "barley", "rye", "spelt", "semolina", "farro",
                "kamut", "bulgur", "einkorn", "pasta", "bread", "noodle")
            && !Contains("buckwheat"))
        {
            AddTag("Contains Gluten");
        }

        if (ContainsAny("oat", "oats"))
        {
            AddTag("Contains Gluten");
        }

        if (ContainsAny(
                "almond", "cashew", "hazelnut", "pecan", "pistachio",
                "walnut", "macadamia", "brazil nut"))
        {
            AddTag("Contains Tree Nuts");
        }

        if (Contains("peanut"))
        {
            AddTag("Contains Peanuts");
        }

        if (Contains("egg"))
        {
            AddTag("Contains Egg");
        }

        if (ContainsAny("soy", "tofu", "edamame", "miso", "tempeh"))
        {
            AddTag("Contains Soy");
        }

        if (ContainsAny(
                "sugar", "syrup", "honey", "molasses",
                "caramel", "fructose", "glucose", "corn syrup"))
        {
            AddTag("Contains Added Sugar");
        }

        if (ContainsAny(
                "wine", "beer", "rum", "vodka", "whiskey",
                "brandy", "sake", "liqueur", "alcohol"))
        {
            AddTag("Contains Alcohol");
        }

        if (ContainsAny(
                "coffee", "espresso", "caffeine",
                "energy drink", "cola", "tea", "matcha"))
        {
            AddTag("Contains Caffeine");
        }

        if (ContainsAny(
                "instant", "processed", "packaged",
                "powder", "ready-to-eat", "snack"))
        {
            AddTag("Highly Processed");
        }

        return result;
    }
}

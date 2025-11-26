using System;
using System.Collections.Generic;
using Pento.Domain.DietaryTags;

namespace Pento.Domain.DietaryTags;

public static class DietaryTagData
{
    // NUTRITION COMPONENTS

    public static readonly DietaryTag ContainsSugar = DietaryTag.Create(
        name: "Contains Sugar",
        description: "This item contains added or naturally occurring sugar.");

    public static readonly DietaryTag ContainsFat = DietaryTag.Create(
        name: "Contains Fat",
        description: "This item contains fats.");

    public static readonly DietaryTag ContainsSaturatedFat = DietaryTag.Create(
        name: "Contains Saturated Fat",
        description: "This item contains saturated fats.");

    public static readonly DietaryTag ContainsTransFat = DietaryTag.Create(
        name: "Contains Trans Fat",
        description: "This item contains trans fats.");

    public static readonly DietaryTag ContainsCarbohydrates = DietaryTag.Create(
        name: "Contains Carbohydrates",
        description: "This item contains carbohydrates.");

    public static readonly DietaryTag ContainsProtein = DietaryTag.Create(
        name: "Contains Protein",
        description: "This item contains protein.");

    public static readonly DietaryTag ContainsFiber = DietaryTag.Create(
        name: "Contains Fiber",
        description: "This item contains dietary fiber.");

    public static readonly DietaryTag ContainsSodium = DietaryTag.Create(
        name: "Contains Sodium",
        description: "This item contains sodium or salt.");

    public static readonly DietaryTag ContainsOil = DietaryTag.Create(
        name: "Contains Oil",
        description: "This item contains added oils or fats.");

    // DAIRY

    public static readonly DietaryTag ContainsDairy = DietaryTag.Create(
        name: "Contains Dairy",
        description: "This item contains dairy ingredients.");

    public static readonly DietaryTag ContainsMilkProtein = DietaryTag.Create(
        name: "Contains Milk Protein",
        description: "This item contains milk-derived proteins.");

    public static readonly DietaryTag ContainsLactose = DietaryTag.Create(
        name: "Contains Lactose",
        description: "This item contains lactose.");

    // EGGS

    public static readonly DietaryTag ContainsEgg = DietaryTag.Create(
        name: "Contains Egg",
        description: "This item contains egg or egg-derived ingredients.");

    // GLUTEN & GRAINS

    public static readonly DietaryTag ContainsGluten = DietaryTag.Create(
        name: "Contains Gluten",
        description: "This item contains gluten.");

    public static readonly DietaryTag ContainsWheat = DietaryTag.Create(
        name: "Contains Wheat",
        description: "This item contains wheat.");

    public static readonly DietaryTag ContainsBarley = DietaryTag.Create(
        name: "Contains Barley",
        description: "This item contains barley.");

    public static readonly DietaryTag ContainsRye = DietaryTag.Create(
        name: "Contains Rye",
        description: "This item contains rye.");

    // NUTS

    public static readonly DietaryTag ContainsAlmonds = DietaryTag.Create(
        name: "Contains Almonds",
        description: "This item contains almonds.");

    public static readonly DietaryTag ContainsCashews = DietaryTag.Create(
        name: "Contains Cashews",
        description: "This item contains cashews.");

    public static readonly DietaryTag ContainsWalnuts = DietaryTag.Create(
        name: "Contains Walnuts",
        description: "This item contains walnuts.");

    public static readonly DietaryTag ContainsPistachios = DietaryTag.Create(
        name: "Contains Pistachios",
        description: "This item contains pistachios.");

    public static readonly DietaryTag ContainsHazelnuts = DietaryTag.Create(
        name: "Contains Hazelnuts",
        description: "This item contains hazelnuts.");

    public static readonly DietaryTag ContainsPecan = DietaryTag.Create(
        name: "Contains Pecan",
        description: "This item contains pecan nuts.");

    public static readonly DietaryTag ContainsTreeNuts = DietaryTag.Create(
        name: "Contains Tree Nuts",
        description: "This item contains tree nuts.");

    // PEANUTS

    public static readonly DietaryTag ContainsPeanuts = DietaryTag.Create(
        name: "Contains Peanuts",
        description: "This item contains peanuts.");

    // SOY

    public static readonly DietaryTag ContainsSoy = DietaryTag.Create(
        name: "Contains Soy",
        description: "This item contains soy ingredients.");

    // SEAFOOD

    public static readonly DietaryTag ContainsFish = DietaryTag.Create(
        name: "Contains Fish",
        description: "This item contains fish.");

    public static readonly DietaryTag ContainsShellfish = DietaryTag.Create(
        name: "Contains Shellfish",
        description: "This item contains shellfish.");

    public static readonly DietaryTag ContainsCrustaceans = DietaryTag.Create(
        name: "Contains Crustaceans",
        description: "This item contains crustaceans.");

    public static readonly DietaryTag ContainsMollusks = DietaryTag.Create(
        name: "Contains Mollusks",
        description: "This item contains mollusks.");

    // OTHER ALLERGENS

    public static readonly DietaryTag ContainsSesame = DietaryTag.Create(
        name: "Contains Sesame",
        description: "This item contains sesame seeds or sesame oil.");

    public static readonly DietaryTag ContainsMustard = DietaryTag.Create(
        name: "Contains Mustard",
        description: "This item contains mustard.");

    public static readonly DietaryTag ContainsCelery = DietaryTag.Create(
        name: "Contains Celery",
        description: "This item contains celery.");

    public static readonly DietaryTag ContainsLupine = DietaryTag.Create(
        name: "Contains Lupine",
        description: "This item contains lupine ingredients.");

    // SPECIAL INGREDIENTS

    public static readonly DietaryTag ContainsAlcohol = DietaryTag.Create(
        name: "Contains Alcohol",
        description: "This item contains alcohol.");

    public static readonly DietaryTag ContainsCaffeine = DietaryTag.Create(
        name: "Contains Caffeine",
        description: "This item contains caffeine.");

    public static readonly DietaryTag ContainsAdditives = DietaryTag.Create(
        name: "Contains Additives",
        description: "This item contains food additives.");

    public static readonly DietaryTag ContainsArtificialSweeteners = DietaryTag.Create(
        name: "Contains Artificial Sweeteners",
        description: "This item contains artificial sweeteners.");

    public static readonly DietaryTag ContainsPreservatives = DietaryTag.Create(
        name: "Contains Preservatives",
        description: "This item contains preservatives.");

    public static readonly DietaryTag ContainsColoring = DietaryTag.Create(
        name: "Contains Coloring",
        description: "This item contains artificial or natural coloring agents.");

    public static readonly DietaryTag ContainsFlavorEnhancers = DietaryTag.Create(
        name: "Contains Flavor Enhancers",
        description: "This item contains added flavor enhancers.");
}

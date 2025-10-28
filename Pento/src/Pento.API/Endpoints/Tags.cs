﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.API.Endpoints;

internal sealed class Tags
{
    internal const string Users = "Users"; //users/{guid}/subscriptions
    internal const string Households = "Households"; //households/{guid}/subscriptions

    internal const string FoodReferences = "FoodReferences";

    internal const string MealPlans = "MealPlans";
    internal const string MealPlanItems = "MealPlanItems";

    internal const string Units = "Units";

    internal const string Recipes = "Recipes";
    internal const string RecipeIngredients = "RecipeIngredients";
    internal const string RecipeDirections = "RecipeDirections";

    internal const string Storages = "Storages";
    internal const string Compartments = "Compartments";
    internal const string StorageItems = "StorageItems";

    internal const string BlogPosts = "BlogPosts";
    internal const string Comments = "Comments";
    internal const string GiveawayPosts = "GiveawayPosts";
    internal const string GiveawayClaims = "GiveawayClaims";

    internal const string Perks = "Perks";

    internal const string Subscriptions = "Subscriptions"; //subscriptions/{guid}/plans, subscriptions/{guid}/perks
    internal const string Invoices = "Invoices";

    internal const string Badges = "Badges"; //requirement = badges/{guid}/requirements, badges/{guid}/perks

    internal const string PointBalances = "PointBalances";
    internal const string PointCategories = "PointCategories";
}

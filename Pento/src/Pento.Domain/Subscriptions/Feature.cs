using Pento.Domain.Shared;

namespace Pento.Domain.Subscriptions;

public sealed class Feature
{
    public static readonly Feature OCR = new(
        FeatureCode.OCR.ToString(),
        "Receipt Scanning",
        "Automatically extract food item names from photographed receipts.",
        5, TimeUnit.Day
    );

    public static readonly Feature ImageRecognition = new(
        FeatureCode.IMAGE_RECOGNITION.ToString(),
        "Image Scanning",
        "Detect food items from images.",
        5, TimeUnit.Day
    );

    public static readonly Feature AIChef = new(
        FeatureCode.AI_CHEF.ToString(),
        "AI Chef",
        "Generate personalized recipes.",
        null, null
    );

    public static readonly Feature StorageSlot = new(
        FeatureCode.STORAGE_SLOT.ToString(),
        "Storage Slot",
        "Total storage slots for pantry management.",
        5, null
    );

    public static readonly Feature MealPlanSlot = new(
        FeatureCode.MEAL_PLAN_SLOT.ToString(),
        "Meal Plan Slot",
        "Total meal plan slot for scheduling and tracking meals.",
        5, null
    );
    public Feature(string code, string name, string description, int? defaultQuota, TimeUnit? defaultResetPeriod)
    {
        Code = code;
        Name = name;
        Description = description;
        DefaultQuota = defaultQuota;
        DefaultResetPeriod = defaultResetPeriod;
    }

    private Feature() { }
    public string Code { get; private set; } //business rule
    public string Name { get; private set; }
    public string Description {  get; private set; }
    public int? DefaultQuota{ get; private set; }
    public TimeUnit? DefaultResetPeriod { get; private set; }

    public void UpdateDetails(string? name, string? description, int? defaultQuota, TimeUnit? defaultResetPeriod)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
        if (!string.IsNullOrWhiteSpace(description))
        {
            Description = description;
        }
        if (defaultQuota.HasValue)
        {
            DefaultQuota = defaultQuota;
        }
        if (defaultResetPeriod.HasValue && defaultQuota.HasValue)
        {
            DefaultResetPeriod = defaultResetPeriod;
        }
    }
}
public enum FeatureCode
{
    OCR,
    IMAGE_RECOGNITION,
    AI_CHEF,
    STORAGE_SLOT,
    MEAL_PLAN_SLOT
}

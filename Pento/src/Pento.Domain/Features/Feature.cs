using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.Features;

public sealed class Feature : BaseEntity
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
    public static readonly Feature GroceryMap = new(
        FeatureCode.GROCERY_MAP.ToString(),
        "Grocery Map",
        "Show grocery options nearby on google map.",
        null, null
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
    public string Code { get; private set; }
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
        Raise(new FeatureUpdatedDomainEvent(Code));
        
    }
}
public enum FeatureCode
{
    OCR,
    IMAGE_RECOGNITION,
    AI_CHEF,
    GROCERY_MAP
}

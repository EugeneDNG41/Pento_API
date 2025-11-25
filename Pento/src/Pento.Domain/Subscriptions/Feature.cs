namespace Pento.Domain.Subscriptions;

public sealed class Feature
{
    public static readonly Feature OCR = new("Receipt Scanning");
    public static readonly Feature ImageRecognition = new("Image Scanning");
    public static readonly Feature AIChef = new("AI Chef");
    public static readonly Feature StorageSlot = new("Storage Slot");
    public static readonly Feature MealPlanSlot = new("Meal Plan Slot");
    private Feature(string name)
    {
        Name = name;
    }
    private Feature() { }
    
    public string Name { get; private set; }
}

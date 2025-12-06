namespace Pento.Application.Abstractions.External.Vision;

public sealed class VisionAnalysisResponse
{
    public string? Caption { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<VisionObjectDto> Objects { get; set; } = new();
}

public sealed class VisionObjectDto
{
    public string Name { get; set; } = default!;
    public double Confidence { get; set; }
    public VisionBoundingBoxDto BoundingBox { get; set; } = new();
}

public sealed class VisionBoundingBoxDto
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

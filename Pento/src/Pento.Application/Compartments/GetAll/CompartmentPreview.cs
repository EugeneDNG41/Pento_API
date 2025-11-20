namespace Pento.Application.Compartments.GetAll;

public sealed record CompartmentPreview
{
    public Guid CompartmentId { get; init; }
    public string CompartmentName { get; init; }
    public int TotalItems { get; init; }
}

namespace Pento.Application.Activities.GetAll;

public sealed record ActivityResponse
{
    public string ActivityCode { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
}

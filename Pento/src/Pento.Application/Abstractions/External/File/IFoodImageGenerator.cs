namespace Pento.Application.Abstractions.External.File;

public interface IFoodImageGenerator
{
    Task<string?> GenerateImageAsync(Guid foodId, CancellationToken ct);
}

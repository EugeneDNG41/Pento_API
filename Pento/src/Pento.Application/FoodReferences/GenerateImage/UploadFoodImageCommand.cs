using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodReferences.GenerateImage;

public sealed record UploadFoodImageCommand(Guid FoodReferenceId, Uri ImageUrl)
    : ICommand<string>;


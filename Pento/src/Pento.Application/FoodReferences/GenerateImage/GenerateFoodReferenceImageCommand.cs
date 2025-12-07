using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodReferences.GenerateImage;

public sealed record GenerateFoodReferenceImageCommand(Guid FoodReferenceId) : ICommand<string>;

using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodReferences.GenerateImage;

public sealed record GenerateAllFoodReferenceImagesCommand(int Limit) : ICommand<int>;

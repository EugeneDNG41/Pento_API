using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.Messaging;


namespace Pento.Application.FoodReferences.Import;

public sealed record ImportFoodReferencesCommand(IFormFile File) : ICommand<int>;


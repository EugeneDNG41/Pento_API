using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.FoodItems.UploadImage;

public record class UploadFoodItemImageCommand(Guid Id, IFormFile? File) : ICommand<Uri>;

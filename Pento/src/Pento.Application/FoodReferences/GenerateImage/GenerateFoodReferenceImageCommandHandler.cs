using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.File;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;

namespace Pento.Application.FoodReferences.GenerateImage;
internal sealed class GenerateFoodReferenceImageCommandHandler(
    IFoodReferenceRepository foodReferenceRepository,
    IPixabayImageService pixabayService,
    IBlobService blobService,
    IUnitOfWork unitOfWork
)
: ICommandHandler<GenerateFoodReferenceImageCommand, string>
{
    public async Task<Result<string>> Handle(
        GenerateFoodReferenceImageCommand request,
        CancellationToken cancellationToken)
    {
        FoodReference? foodRef = await foodReferenceRepository.GetByIdAsync(request.FoodReferenceId, cancellationToken);
        if (foodRef is null)
        {
            return Result.Failure<string>(FoodReferenceErrors.NotFound);
        }

        string cleanedName = CleanName(foodRef.Name);
        string query = cleanedName;

        Result<Uri> imageResult = await pixabayService.GetImageUrlAsync(query, cancellationToken);
        if (imageResult.IsFailure)
        {
            return Result.Failure<string>(imageResult.Error);
        }

        using var httpClient = new HttpClient();
        using Stream imageStream = await httpClient.GetStreamAsync(imageResult.Value, cancellationToken);

        string safeName = cleanedName.Replace(" ", "_");
        string fileName = $"{foodRef.Id}_{safeName}.jpg";

        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream, cancellationToken);

        memoryStream.Position = 0;

        var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };

        Result<Uri> uploadResult = await blobService.UploadImageAsync(formFile, "foodreference", cancellationToken);

        if (uploadResult.IsFailure)
        {
            return Result.Failure<string>(uploadResult.Error);
        }

        foodRef.UpdateImageUrl(uploadResult.Value, DateTime.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return uploadResult.Value.AbsoluteUri;
    }

    private static string CleanName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        name = name.Trim();

        while (name.Contains("  "))
        {
            name = name.Replace("  ", " ");
        }

        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach (char ch in invalidChars)
        {
            name = name.Replace(ch.ToString(), "");
        }

        return name;
    }
}

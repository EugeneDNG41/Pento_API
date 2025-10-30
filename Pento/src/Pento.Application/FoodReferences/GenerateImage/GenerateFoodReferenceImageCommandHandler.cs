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
            return Result.Failure<string>(FoodReferenceErrors.NotFound(request.FoodReferenceId));
        }

        string query = $"{foodRef.Name} {foodRef.Notes}".Trim();
        Result<Uri> unsplashResult = await pixabayService.GetImageUrlAsync(query, cancellationToken);
        if (unsplashResult.IsFailure)
        {
            return Result.Failure<string>(unsplashResult.Error);
        }

        using var httpClient = new HttpClient();
        using Stream imageStream = await httpClient.GetStreamAsync(unsplashResult.Value, cancellationToken);

        string fileName = $"{foodRef.Id}_{foodRef.Name}.jpg";
        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;
        var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };


        Result<string> uploadResult = await blobService.UploadImageAsync(formFile, cancellationToken);
        if (uploadResult.IsFailure)
        {
            return Result.Failure<string>(uploadResult.Error);
        }
        foodRef.UpdateImageUrl(new Uri(uploadResult.Value), DateTime.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(uploadResult.Value);
    }
}

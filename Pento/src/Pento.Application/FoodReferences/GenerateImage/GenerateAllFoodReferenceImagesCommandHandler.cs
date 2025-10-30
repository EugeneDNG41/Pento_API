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
internal sealed class GenerateAllFoodReferenceImagesCommandHandler(
    IFoodReferenceRepository foodReferenceRepository,
    IPixabayImageService pixabayService,
    IBlobService blobService,
    IUnitOfWork unitOfWork)
    : ICommandHandler<GenerateAllFoodReferenceImagesCommand, int>
{
    public async Task<Result<int>> Handle(
        GenerateAllFoodReferenceImagesCommand request,
        CancellationToken cancellationToken)
    {
        int limit = Math.Clamp(request.Limit, 1, 50);

        IReadOnlyList<FoodReference> foods = await foodReferenceRepository
            .GetAllWithoutImageAsync(limit, cancellationToken);

        if (foods.Count == 0)
        {
            return Result.Success(0);
        }

        int successCount = 0;

        foreach (FoodReference foodRef in foods)
        {
            try
            {
                string query =
                    $"{foodRef.Name} {foodRef.Notes}"
                    .Trim();

                Result<Uri> imgResult = await pixabayService.GetImageUrlAsync(query, cancellationToken);
                if (imgResult.IsFailure)
                {
                    continue;
                }

                using var httpClient = new HttpClient();
                using Stream imageStream = await httpClient.GetStreamAsync(imgResult.Value, cancellationToken);

                string fileName = $"{foodRef.Id}_{foodRef.Name}.jpg";
                using var memoryStream = new MemoryStream();
                await imageStream.CopyToAsync(memoryStream, cancellationToken);
                memoryStream.Position = 0;

                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                };

                Result<string> uploadResult =
                    await blobService.UploadFileAsync(formFile, "images", cancellationToken);
                if (uploadResult.IsFailure)
                {
                    continue;
                }

                foodRef.UpdateImageUrl(new Uri(uploadResult.Value), DateTime.UtcNow);
                successCount++;
            }
            catch
            {
                //continue
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(successCount);
    }
}

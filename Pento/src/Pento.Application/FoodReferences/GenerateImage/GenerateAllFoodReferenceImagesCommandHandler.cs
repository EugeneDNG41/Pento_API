using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.External.File;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;


namespace Pento.Application.FoodReferences.GenerateImage;

internal sealed class GenerateAllFoodReferenceImagesCommandHandler(
    IGenericRepository<FoodReference> foodRepo,
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

        var foods = (await foodRepo.FindAsync(
            fr => fr.ImageUrl == null,
            cancellationToken
        )).Take(limit).ToList();



        if (foods.Count == 0)
        {
            return Result.Success(0);
        }

        int successCount = 0;

        foreach (FoodReference foodRef in foods)
        {
            try
            {
                string query = foodRef.Name.Trim();

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

                Result<Uri> uploadResult =
                    await blobService.UploadImageAsync(formFile, "foodreference", cancellationToken);

                if (uploadResult.IsFailure)
                {
                    continue;
                }

                foodRef.UpdateImageUrl(uploadResult.Value, DateTime.UtcNow);
                successCount++;

            }
            catch
            {
                //Eror
            }
        }
        await foodRepo.UpdateRangeAsync(foods, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(successCount);
    }
}

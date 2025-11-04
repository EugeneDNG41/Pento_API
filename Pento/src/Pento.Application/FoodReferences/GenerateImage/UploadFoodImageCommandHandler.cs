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
internal sealed class ManualUploadFoodImageCommandHandler(
    IFoodReferenceRepository foodReferenceRepository,
    IBlobService blobService,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UploadFoodImageCommand, string>
{
    public async Task<Result<string>> Handle(
        UploadFoodImageCommand request,
        CancellationToken cancellationToken)
    {
        FoodReference? foodRef = await foodReferenceRepository.GetByIdAsync(request.FoodReferenceId, cancellationToken);
        if (foodRef is null)
        {
            return Result.Failure<string>(FoodReferenceErrors.NotFound);
        }

        try
        {
            using var httpClient = new HttpClient();
            using Stream imageStream = await httpClient.GetStreamAsync(request.ImageUrl, cancellationToken);

            string fileName = $"{foodRef.Id}_{foodRef.Name}.jpg";
            using var memoryStream = new MemoryStream();
            await imageStream.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            Result<string> uploadResult = await blobService.UploadImageAsync(formFile, "foodreference", cancellationToken);
            if (uploadResult.IsFailure)
            {
                return Result.Failure<string>(uploadResult.Error);
            }

            foodRef.UpdateImageUrl(new Uri(uploadResult.Value), DateTime.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(uploadResult.Value);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(
                Error.Problem("ManualUpload.Failed", $"Failed to upload image: {ex.Message}")
            );
        }
    }
}

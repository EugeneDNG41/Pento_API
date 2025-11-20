using System.Globalization;
using GenerativeAI;
using GenerativeAI.Types;
using OpenFoodFacts4Net.ApiClient;
using OpenFoodFacts4Net.Json.Data;
using Pento.Application.Abstractions.Data;
using Pento.Application.Abstractions.OpenFoodFacts;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;

namespace Pento.Infrastructure.OpenFoodFacts;

internal sealed class BarcodeService(GeminiModel model, IGenericRepository<FoodReference> foodReferenceRepository, IUnitOfWork unitOfWork) : IBarcodeService
{
    public async Task<Result<FoodReference>> FetchProductAsync(string barcode, CancellationToken cancellationToken)
    {
        try
        {
            string userAgent = UserAgentHelper.GetUserAgent("Pento", ".Net Platform", "1.0", null);
            var client = new Client(userAgent);
            GetProductResponse product = await client.GetProductAsync(barcode);
            if (product.Status == 0 || product.Product is null)
            {
                return Result.Failure<FoodReference>(BarcodeServiceErrors.ProductNotFound);
            }
            string? brandName = product.Product.BrandsTags.FirstOrDefault();
            var foodGroupsBuilder = new System.Text.StringBuilder();
            var unitTypesBuilder = new System.Text.StringBuilder();
            foreach (FoodGroup fg in Enum.GetValues<FoodGroup>())
            {
                foodGroupsBuilder.Append(CultureInfo.InvariantCulture, $"{(int)fg}: {fg}");
            }
            foreach (UnitType ut in Enum.GetValues<UnitType>())
            {
                unitTypesBuilder.Append(CultureInfo.InvariantCulture, $"{(int)ut}: {ut}");
            }
            string foodGroups = foodGroupsBuilder.ToString();
            string unitTypes = unitTypesBuilder.ToString();

            var parts = new List<Part>
            {
                new Part { Text = "You are a professional food storage and safety expert." },
                new Part { Text = $"Provide information regarding {product.Product.ProductName}'s food group, unit type, and safe and realistic average number of days {product.Product.ProductName} can be stored in a pantry, fridge and freezer respectively" },
                new Part { Text = "Ensure the following hierarchy holds: pantry <= fridge <= freeezer" },
                new Part { Text = $"Possible food groups: {foodGroups}"},
                new Part { Text = $"Possible unit types: {unitTypes}"},
            };

            ProductExtraInformation? extraInfo = await model.GenerateObjectAsync<ProductExtraInformation>(parts, cancellationToken);
            if (extraInfo != null)
            {
                var foodReference = FoodReference.Create(
                    name: product.Product.ProductName ?? product.Product.GenericName,
                    foodGroup: (FoodGroup)extraInfo.FoodGroup,
                    foodCategoryId: null,
                    brand: brandName is not null ? Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(brandName) : null,
                    barcode: barcode,
                    usdaId: string.Empty,
                    typicalShelfLifeDaysPantry: extraInfo.PantryShelfLife,
                    typicalShelfLifeDaysFridge: extraInfo.FridgeShelfLife,
                    typicalShelfLifeDaysFreezer: extraInfo.FreezerShelfLife,
                    addedBy: null,
                    imageUrl: product.Product.ImageUrl != null ? new Uri(product.Product.ImageUrl) : null,
                    unitType: (UnitType)extraInfo.UnitType,
                    utcNow: DateTime.UtcNow
                );
                foodReferenceRepository.Add(foodReference);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return foodReference;
            }
            return Result.Failure<FoodReference>(BarcodeServiceErrors.ApiError);
        }
        catch (Exception)
        {
            return Result.Failure<FoodReference>(BarcodeServiceErrors.ApiError);
        }
    }
}
internal sealed record ProductExtraInformation(int FoodGroup, int UnitType, int PantryShelfLife, int FridgeShelfLife, int FreezerShelfLife);


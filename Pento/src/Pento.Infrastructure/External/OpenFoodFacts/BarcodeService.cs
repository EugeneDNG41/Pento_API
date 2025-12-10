using System.Globalization;
using System.Net.Http.Json;
using System.Threading;
using GenerativeAI;
using GenerativeAI.Types;
using Newtonsoft.Json;
using Pento.Application.Abstractions.External.Barcode;
using Pento.Application.Abstractions.Persistence;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;
using Pento.Infrastructure.External.Identity;
using static Dapper.SqlMapper;

namespace Pento.Infrastructure.External.OpenFoodFacts;

internal sealed class BarcodeService(
    GenerativeModel model,
    IGenericRepository<FoodReference> foodReferenceRepository, 
    IUnitOfWork unitOfWork,
    OpenFoodFactsClient client) : IBarcodeService
{
    public async Task<Result<FoodReference>> FetchProductAsync(string barcode, CancellationToken cancellationToken)
    {
        try
        {
            Result<ProductResponse> product = await client.FetchProductByCodeAsync(barcode, cancellationToken);
            if (product.IsFailure)
            {
                return Result.Failure<FoodReference>(product.Error);
            }
            if (product.Value.Status == false || product.Value.Product == null)
            {
                return Result.Failure<FoodReference>(BarcodeServiceErrors.ProductNotFound);
            }
            string? brandName = product.Value.Product.BrandsTags.FirstOrDefault();
            var foodGroupsBuilder = new System.Text.StringBuilder();
            var unitTypesBuilder = new System.Text.StringBuilder();
            foreach (FoodGroup fg in Enum.GetValues<FoodGroup>())
            {
                foodGroupsBuilder.Append(CultureInfo.InvariantCulture, $"{(int)fg}: {fg}, ");
            }
            foreach (UnitType ut in Enum.GetValues<UnitType>())
            {
                unitTypesBuilder.Append(CultureInfo.InvariantCulture, $"{(int)ut}: {ut}, ");
            }
            string foodGroups = foodGroupsBuilder.ToString();
            string unitTypes = unitTypesBuilder.ToString();
            string productName = string.IsNullOrEmpty(product.Value.Product.ProductName) ? product.Value.Product.GenericName : product.Value.Product.ProductName;
            string productKeywords = product.Value.Product.Keywords.Length > 0 ? string.Join(", ", product.Value.Product.Keywords) : string.Empty;
            string productNameOrKeywords = !string.IsNullOrEmpty(productName) ? productName : productKeywords;
            var parts = new List<Part>
            {
                new Part { Text = $"Provide information regarding following product's food group, readable name, unit type, and safe and realistic average number of days it can be stored in a pantry, fridge and freezer respectively" },
                new Part { Text = $"Product Name or Keywords: {productNameOrKeywords}"},
                new Part { Text = "Prioritize  Ensure the following hierarchy holds: pantry <= fridge <= freeezer" },
                new Part { Text = $"Possible food groups: {foodGroups}"},
                new Part { Text = $"Possible unit types: {unitTypes}"}
            };
            ProductExtraInformation? extraInfo = await model.GenerateObjectAsync<ProductExtraInformation>(parts, cancellationToken);
            if (extraInfo != null)
            {
                var foodReference = FoodReference.Create(
                    name: extraInfo.ReadableName,
                    foodGroup: (FoodGroup)extraInfo.FoodGroup,
                    foodCategoryId: null,
                    brand: brandName is not null ? Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(brandName) : null,
                    barcode: barcode,
                    usdaId: string.Empty,
                    typicalShelfLifeDaysPantry: extraInfo.PantryShelfLife,
                    typicalShelfLifeDaysFridge: extraInfo.FridgeShelfLife,
                    typicalShelfLifeDaysFreezer: extraInfo.FreezerShelfLife,
                    addedBy: null,
                    imageUrl: product.Value.Product.ImageUrl != null ? new Uri(product.Value.Product.ImageUrl) : null,
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
internal sealed record ProductExtraInformation(string ReadableName, int FoodGroup, int UnitType, int PantryShelfLife, int FridgeShelfLife, int FreezerShelfLife);

public class OpenFoodFactsClient(HttpClient httpClient)
{
    public async Task<Result<ProductResponse>> FetchProductByCodeAsync(string code, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await httpClient.GetAsync(
            $"product/{code}.json",
            cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            // Handle error or throw an exception
            return Result.Failure<ProductResponse>(BarcodeServiceErrors.ApiError);
        }


        string content = await response.Content.ReadAsStringAsync(cancellationToken);
        ProductResponse? product = JsonConvert.DeserializeObject<ProductResponse>(content);
        if (product == null)
        {
            return Result.Failure<ProductResponse>(BarcodeServiceErrors.SerializationError);
        }
        return product;
    }
}

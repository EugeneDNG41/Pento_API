
using Newtonsoft.Json;
using System;

namespace Pento.Application.Abstractions.External.Barcode;

public class ProductResponse
{
    public Product Product { get; set; }
    public string Code { get; set; }
    public bool? Status { get; set; }

    [JsonProperty("status_verbose")]
    public string StatusVerbose { get; set; }
}
public class SelectedImages
{
    public SelectedImage Front { get; set; }
    public SelectedImage Ingredients { get; set; }
    public SelectedImage Nutrition { get; set; }
}
public class SelectedImage
{
    public SelectedImageItem Display { get; set; }
    public SelectedImageItem Small { get; set; }
    public SelectedImageItem Thumb { get; set; }
}
public class SelectedImageItem
{
    public string En { get; set; }
    public string Fr { get; set; }
    public string Pl { get; set; }
}
public class Product
{
    [JsonProperty("selected_images")]
    public SelectedImages SelectedImages { get; set; }
    public string Brands { get; set; }
    [JsonProperty("brands_tags")]
    public string[] BrandsTags { get; set; }
    [JsonProperty("categories_tags")]
    public string[] CategoriesTags { get; set; }
    [JsonProperty("expiration_date")]
    public string ExpirationDate { get; set; }
    [JsonProperty("generic_name")]
    public string GenericName { get; set; }
    [JsonProperty("image_front_small_url")]
    public string ImageFrontSmallUrl { get; set; }
    [JsonProperty("image_front_thumb_url")]
    public string ImageFrontThumbUrl { get; set; }
    [JsonProperty("image_front_url")]
    public string ImageFrontUrl { get; set; }
    [JsonProperty("image_small_url")]
    public string ImageSmallUrl { get; set; }
    [JsonProperty("image_thumb_url")]
    public string ImageThumbUrl { get; set; }
    [JsonProperty("image_url")]
    public string ImageUrl { get; set; }
    [JsonProperty("ingredients_tags")]
    public string[] IngredientsTags { get; set; }
    [JsonProperty("ingredients_text")]
    public string IngredientsText { get; set; }
    [JsonProperty("ingredients_text_with_allergens")]
    public string IngredientsTextWithAllergens { get; set; }
    [JsonProperty("_keywords")]
    public string[] Keywords { get; set; }
    [JsonProperty("languages_tags")]
    public string[] LanguagesTags { get; set; }
    [JsonProperty("net_weight_unit")]
    public string NetWeightUnit { get; set; }
    [JsonProperty("net_weight_value")]
    public string NetWeightValue { get; set; }
    [JsonProperty("product_name")]
    public string ProductName { get; set; }
    [JsonProperty("product_quantity")]
    public string ProductQuantity { get; set; }
    [JsonProperty("product_quantity_unit")]
    public string ProductQuantityUnit { get; set; }
    [JsonProperty("purchase_places")]
    public string PurchasePlaces { get; set; }
    [JsonProperty("purchase_places_tags")]
    public string[] PurchasePlacesTags { get; set; }
    [JsonProperty("quality_tags")]
    public string[] QualityTags { get; set; }
    [JsonProperty("serving_quantity")]
    public string ServingQuantity { get; set; }
    [JsonProperty("serving_size")]
    public string ServingSize { get; set; }

}

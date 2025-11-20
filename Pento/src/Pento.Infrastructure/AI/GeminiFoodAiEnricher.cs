using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GenerativeAI;
using GenerativeAI.Types;
using Microsoft.Extensions.Configuration;
using OpenFoodFacts4Net.ApiClient;
using OpenFoodFacts4Net.Json.Data;
using Pento.Application.Abstractions.File;
using Pento.Application.Abstractions.OpenFoodFacts;
using Pento.Application.FoodReferences.Enrich;
using Pento.Domain.Abstractions;
using Pento.Domain.FoodReferences;
using Pento.Domain.Units;
using Pento.Infrastructure.OpenFoodFacts;
using Pento.Infrastructure.Repositories;

namespace Pento.Infrastructure.AI;

internal sealed class GeminiFoodAiEnricher(HttpClient http, IConfiguration config, GeminiModel model) : IFoodAiEnricher
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public async Task<Result<ProductExtraInformationWithoutFoodGroup>> EnrichAsync(string foodName, CancellationToken cancellationToken)
    {
        try
        {
            var unitTypesBuilder = new System.Text.StringBuilder();
            foreach (UnitType ut in Enum.GetValues<UnitType>())
            {
                unitTypesBuilder.Append(CultureInfo.InvariantCulture, $"{(int)ut}: {ut}");
            }
            string unitTypes = unitTypesBuilder.ToString();

            var parts = new List<Part>
            {
                new Part { Text = "You are a professional food storage and safety expert." },
                new Part { Text = $"Provide information regarding {foodName}'s unit type and safe and realistic average number of days {foodName} can be stored in a pantry, fridge and freezer respectively" },
                new Part { Text = "Ensure the following hierarchy holds: pantry <= fridge <= freeezer" },
                new Part { Text = $"Possible unit types: {unitTypes}"},
            };

            ProductExtraInformationWithoutFoodGroup? extraInfo = await model.GenerateObjectAsync<ProductExtraInformationWithoutFoodGroup>(parts, cancellationToken);
            if (extraInfo != null)
            {
                return extraInfo;
            }
            return Result.Failure<ProductExtraInformationWithoutFoodGroup>(FoodAiEnricherErrors.ApiError);
        }
        catch (Exception)
        {
            return Result.Failure<ProductExtraInformationWithoutFoodGroup>(FoodAiEnricherErrors.ApiError);
        }
    }
    public async Task<FoodEnrichmentResult> EnrichAsync(FoodEnrichmentAsk ask, CancellationToken ct)
    {
        string apiKey = config["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini:ApiKey missing");

        string modelUrl =
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

        string prompt = $@"
            You are a professional food storage and safety expert.
            Given the following food details, respond STRICTLY with a compact JSON object only (no explanations, no markdown)
            in the following format:
            {{
              ""shelf_life_days"": {{
                ""pantry"": <integer>,
                ""fridge"": <integer>,
                ""freezer"": <integer>
              }}
            }}
            Each value should represent a safe and realistic average number of days the food can be stored. 
            **Crucially, ensure the following hierarchy holds: freezer >= fridge >= pantry.**

            Name: {ask.Name}
            Group: {ask.FoodGroup}
            ";

        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[] { new { text = prompt } }
                }
            }
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, modelUrl)
        {
            Content = JsonContent.Create(body)
        };

        HttpResponseMessage res = await http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();

        GeminiResponse? root = await res.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: ct);
        string text = root?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? string.Empty;

        text = CleanGeminiOutput(text);

        var dto = new ShelfLifeDto();
        try
        {
            dto = JsonSerializer.Deserialize<ShelfLifeDto>(text, JsonOpts) ?? new ShelfLifeDto();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON parse failed: {ex.Message}");
            Console.WriteLine($"Raw text: {text}");
            dto.shelf_life_days = new ShelfLifeDays { pantry = 0, fridge = 0, freezer = 0 };
        }

        return new FoodEnrichmentResult(
            Id: Guid.NewGuid(),
            TypicalShelfLifeDays_Pantry: dto.shelf_life_days?.pantry ?? 0,
            TypicalShelfLifeDays_Fridge: dto.shelf_life_days?.fridge ?? 0,
            TypicalShelfLifeDays_Freezer: dto.shelf_life_days?.freezer ?? 0
        );
    }

    private static string CleanGeminiOutput(string text)
    {
        if (text.Contains("```", StringComparison.Ordinal))
        {
            int start = text.IndexOf("```", StringComparison.Ordinal);
            int end = text.LastIndexOf("```", StringComparison.Ordinal);
            if (end > start)
            {
                text = text.Substring(start + 3, end - start - 3);
                text = text.Replace("json", "", StringComparison.OrdinalIgnoreCase);
            }
        }

        text = text.Trim();

        if (!text.StartsWith('{'))
        {
            int braceIndex = text.IndexOf('{');
            if (braceIndex >= 0)
            {
                text = text.Substring(braceIndex);
            }
        }

        if (!text.EndsWith('}'))
        {
            int braceEnd = text.LastIndexOf('}');
            if (braceEnd > 0)
            {
                text = text.Substring(0, braceEnd + 1);
            }
        }

        return text.Trim();
    }


    private sealed class ShelfLifeDto
    {
        public ShelfLifeDays? shelf_life_days { get; set; }
    }

    private sealed class ShelfLifeDays
    {
        public int pantry { get; set; }
        public int fridge { get; set; }
        public int freezer { get; set; }
    }

    private sealed class GeminiResponse
    {
        public List<Candidate>? Candidates { get; set; } = null!;
    }

    private sealed class Candidate
    {
        public Content? Content { get; set; } = null!;
    }

    private sealed class Content
    {
        public List<Part>? Parts { get; set; } = null!;
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Pento.Application.FoodReferences.Enrich;

namespace Pento.Infrastructure.AI;

internal sealed class GeminiFoodAiEnricher(HttpClient http, IConfiguration config) : IFoodAiEnricher
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<FoodEnrichmentResult> EnrichAsync(FoodEnrichmentAsk ask, CancellationToken ct)
    {
        string apiKey = config["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini:ApiKey missing");

        // STEP 1: Get summary + expiry days
        string infoUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";
        string prompt = $@"
You are an expert food data enricher. Given the food item details, return ONLY a compact JSON object with:
- short_name (string, ≤3 words)
- suggested_expiry_days (int, realistic shelf-life)
Return strictly JSON, no markdown or explanation.

Name: {ask.Name}
Group: {ask.FoodGroup}
DataType: {ask.DataType}
";

        var body = new
        {
            contents = new[]
            {
                new {
                    parts = new[] { new { text = prompt } }
                }
            }
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, infoUrl)
        {
            Content = JsonContent.Create(body)
        };

        HttpResponseMessage res = await http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();
        GeminiResponse? root = await res.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: ct);
        string text = root?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? string.Empty;

        text = CleanGeminiOutput(text);
        var dto = new AiDto();
        try
        {
            dto = JsonSerializer.Deserialize<AiDto>(text, JsonOpts) ?? new AiDto();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"⚠️ JSON parse failed: {ex.Message}");
            Console.WriteLine($"Raw text: {text}");
            dto.short_name = "ParseError";
        }

        // STEP 2: Generate realistic image
        string imageModelUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-preview-image-generation:generateContent?key={apiKey}";
        string imagePrompt = $"Generate a realistic food photo of {dto.short_name ?? ask.Name}, studio lighting, clean background.";
        var imgBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = imagePrompt } } }
            }
        };

        string? generatedImageUrl = null;
        try
        {
            HttpResponseMessage imgRes = await http.PostAsJsonAsync(imageModelUrl, imgBody, ct);
            imgRes.EnsureSuccessStatusCode();
            JsonElement imgJson = await imgRes.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);

            // Gemini image API returns base64 sometimes:
            if (imgJson.TryGetProperty("candidates", out JsonElement candidates) &&
                candidates[0].GetProperty("content").GetProperty("parts")[0].TryGetProperty("inline_data", out JsonElement inlineData))
            {
                // Convert base64 to temporary data URL (or send to blob service later)
                generatedImageUrl = "data:image/jpeg;base64," + inlineData.GetProperty("data").GetString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Image generation failed: {ex.Message}");
        }

        // STEP 3: Return full enrichment
        return new FoodEnrichmentResult(
            ShortName: dto.short_name ?? ask.Name,
            SuggestedExpiryDays: dto.suggested_expiry_days ?? 0,
            ImageUrl: generatedImageUrl != null ? new Uri(generatedImageUrl) : null
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

    private sealed class AiDto
    {
        public string? short_name { get; set; }
        public int? suggested_expiry_days { get; set; } = null!;
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

    private sealed class Part
    {
        public string? Text { get; set; } = null!;
    }
}

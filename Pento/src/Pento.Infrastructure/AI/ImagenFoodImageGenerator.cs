using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pento.Application.Abstractions.File;

namespace Pento.Infrastructure.AI;
internal sealed class ImagenFoodImageGenerator(HttpClient http, IConfiguration config, ApplicationDbContext db)
    : IFoodImageGenerator
{
    public async Task<string?> GenerateImageAsync(Guid foodId, CancellationToken ct)
    {
        Domain.FoodReferences.FoodReference? food = await db.FoodReferences
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == foodId, ct);

        if (food is null)
        {
            Console.WriteLine($"⚠️ FoodReference with ID {foodId} not found.");
            return null;
        }

        string apiKey = config["Gemini:ApiKey"]
            ?? throw new InvalidOperationException("Gemini:ApiKey missing");

        string model = "imagen-3.0-generate-002"; //"imagen-4.0-generate-preview-06-06"
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:predict?key={apiKey}";

        string prompt = $"A high-quality, realistic food photography of {food.Name}. Studio lighting, bright background, soft shadow, centered composition.";

        var body = new
        {
            instances = new[]
            {
                new { prompt }
            }
        };

        Console.WriteLine($"🧠 Requesting Imagen for: {food.Name}");

        HttpResponseMessage res = await http.PostAsJsonAsync(url, body, ct);
        Console.WriteLine($"Imagen response: {res.StatusCode}");

        if (!res.IsSuccessStatusCode)
        {
            string err = await res.Content.ReadAsStringAsync(ct);
            Console.WriteLine($"Imagen API failed: {res.StatusCode} - {err}");
            return null;
        }

        JsonElement json = await res.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);

        try
        {
            string? base64 = json
                .GetProperty("predictions")[0]
                .GetProperty("bytesBase64Encoded")
                .GetString();

            Console.WriteLine($"✅ Imagen generated image for {food.Name}");
            return base64;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Failed to extract base64: {ex.Message}");
            Console.WriteLine(json.ToString());
            return null;
        }
    }
}

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; // <-- Added for IConfiguration

namespace AutomatedContentGuard.Services
{
    public class GeminiModerationResult
    {
        public bool IsFlagged { get; set; }
        public double ToxicityScore { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class GeminiModerationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration; // <-- Added field

        // Constructor now receives IConfiguration via Dependency Injection
        public GeminiModerationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<GeminiModerationResult> AnalyzeTextAsync(string text)
        {
            // 1. Reads from Cloud Env Variable first, falls back to appsettings.json
            string apiKey = Environment.GetEnvironmentVariable("HuggingFace__ApiKey")
                         ?? _configuration["HuggingFace:ApiKey"]
                         ?? string.Empty;

            var url = "https://router.huggingface.co/hf-inference/models/KoalaAI/Text-Moderation";
            var requestBody = new { inputs = text };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };

            // Use the dynamically loaded apiKey variable
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Hugging Face Error [{response.StatusCode}]: {responseString}");
                }

                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                bool isFlagged = false;
                double maxToxicityScore = 0.0;
                string detectedCategory = "Safe";

                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var categories = root[0];
                    if (categories.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var category in categories.EnumerateArray())
                        {
                            string label = category.GetProperty("label").GetString() ?? "";
                            double score = category.GetProperty("score").GetDouble();

                            if (!label.Equals("OK", StringComparison.OrdinalIgnoreCase) &&
                                !label.Equals("clean", StringComparison.OrdinalIgnoreCase))
                            {
                                if (score > maxToxicityScore)
                                {
                                    maxToxicityScore = score;
                                    detectedCategory = label;
                                }

                                if (score > 0.3)
                                {
                                    isFlagged = true;
                                }
                            }
                        }
                    }
                }

                double finalScore = Math.Round(maxToxicityScore * 10, 1);

                return new GeminiModerationResult
                {
                    IsFlagged = isFlagged,
                    ToxicityScore = finalScore,
                    Reason = isFlagged ? $"AI Flagged category: {detectedCategory}" : "Content is safe"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Moderation Failed: {ex.Message}");
            }
        }
    }
}
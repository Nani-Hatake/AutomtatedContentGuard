using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
        private readonly IConfiguration _configuration;

        public GeminiModerationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<GeminiModerationResult> AnalyzeTextAsync(string text)
        {
            // 1. Get API Key from Environment Variable or appsettings
            string apiKey = Environment.GetEnvironmentVariable("HuggingFace__ApiKey")
                         ?? _configuration["HuggingFace:ApiKey"]
                         ?? string.Empty;

            // CORRECT Hugging Face Inference API URL
            var url = "https://api-inference.huggingface.co/models/KoalaAI/Text-Moderation";
            var requestBody = new { inputs = text };

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
                };

                if (!string.IsNullOrEmpty(apiKey))
                {
                    request.Headers.Add("Authorization", $"Bearer {apiKey}");
                }

                var response = await _httpClient.SendAsync(request);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[Hugging Face Warning {response.StatusCode}]: {responseString}");
                    // Safe fallback if API key is invalid or Hugging Face is warming up
                    return GetFallbackResult("AI API temporarily unavailable or warming up.");
                }

                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                bool isFlagged = false;
                double maxToxicityScore = 0.0;
                string detectedCategory = "Safe";

                // Parse nested array response format: [[{"label": "...", "score": 0.X}]]
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
                Console.WriteLine($"[Moderation Exception]: {ex.Message}");
                return GetFallbackResult("Local fallback score returned due to an exception.");
            }
        }

        private static GeminiModerationResult GetFallbackResult(string reason)
        {
            return new GeminiModerationResult
            {
                IsFlagged = false,
                ToxicityScore = 0.0,
                Reason = reason
            };
        }
    }
}

using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIAssistant.Api.Services
{
    public class GoogleAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";

        public GoogleAIService(IConfiguration configuration, HttpClient httpClient)
        {
            var apiKey = configuration["GoogleAI:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException(
                    "Google AI API key is not configured.");
            }

            _apiKey = apiKey;
            _httpClient = httpClient;
        }

        public async Task<string> GetResponseAsync(string message)
        {
            var request = new GoogleAIRequest
            {
                Contents = new List<Content>
                {
                    new Content
                    {
                        Role = "user",
                        Parts = new List<Part>
                        {
                            new Part
                            {
                                Text = message
                            }
                        }
                    }
                },
                GenerationConfig = new GenerationConfig
                {
                    Temperature = 0.7f,
                    MaxOutputTokens = 2048,
                    TopP = 0.95f,
                    TopK = 40
                }
            };

            var url = $"{BaseUrl}/gemini-1.5-flash:generateContent?key={_apiKey}";

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(url, content);
            https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key={apiKey}https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContentprivate const string BaseUrl = "https://generativelanguage.googleapis.com/v1/models";response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var googleAIResponse = JsonSerializer.Deserialize<GoogleAIResponse>(responseContent);

            return googleAIResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? string.Empty;
        }
    }

    public class GoogleAIRequest
    {
        [JsonPropertyName("contents")]
        public List<Content> Contents { get; set; } = new();

        [JsonPropertyName("generationConfig")]
        public GenerationConfig? GenerationConfig { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; } = new();
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    public class GenerationConfig
    {
        [JsonPropertyName("temperature")]
        public float Temperature { get; set; }

        [JsonPropertyName("maxOutputTokens")]
        public int MaxOutputTokens { get; set; }

        [JsonPropertyName("topP")]
        public float TopP { get; set; }

        [JsonPropertyName("topK")]
        public int TopK { get; set; }
    }

    public class GoogleAIResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate>? Candidates { get; set; }

        [JsonPropertyName("usageMetadata")]
        public UsageMetadata? UsageMetadata { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content? Content { get; set; }

        [JsonPropertyName("finishReason")]
        public string? FinishReason { get; set; }
    }

    public class UsageMetadata
    {
        [JsonPropertyName("promptTokenCount")]
        public int PromptTokenCount { get; set; }

        [JsonPropertyName("candidatesTokenCount")]
        public int CandidatesTokenCount { get; set; }

        [JsonPropertyName("totalTokenCount")]
        public int TotalTokenCount { get; set; }
    }
}

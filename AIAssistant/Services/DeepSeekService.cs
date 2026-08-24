using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIAssistant.Api.Services
{
    public class DeepSeekService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.deepseek.com/chat/completions";

        public DeepSeekService(IConfiguration configuration, HttpClient httpClient)
        {
            var apiKey = configuration["DeepSeek:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException(
                    "DeepSeek API key is not configured.");
            }

            _apiKey = apiKey;
            _httpClient = httpClient;
        }

        public async Task<string> GetResponseAsync(string message)
        {
            var request = new DeepSeekRequest
            {
                Model = "deepseek-chat",
                Messages = new List<Message>
                {
                    new Message
                    {
                        Role = "user",
                        Content = message
                    }
                },
                Temperature = 0.7f,
                MaxTokens = 2048
            };

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, BaseUrl)
            {
                Content = content
            };

            httpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var deepSeekResponse = JsonSerializer.Deserialize<DeepSeekResponse>(responseContent);

            return deepSeekResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? string.Empty;
        }
    }

    public class DeepSeekRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; } = new();

        [JsonPropertyName("temperature")]
        public float Temperature { get; set; }

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    public class DeepSeekResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice>? Choices { get; set; }

        [JsonPropertyName("usage")]
        public Usage? Usage { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("message")]
        public Message? Message { get; set; }

        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }
    }

    public class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}

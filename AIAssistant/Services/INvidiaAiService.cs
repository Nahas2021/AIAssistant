using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Linq;
using Microsoft.Extensions.Options;

namespace AIAssistant.Api.Services;

public interface INvidiaAiService
{
    Task<string> GenerateChatAsync(IEnumerable<ChatMessage> messages, string? model = null);

    // Backwards-compatible convenience overload
    Task<string> GenerateChatAsync(string prompt, string? model = null);
}

public record ChatMessage(string Role, string Content);

public class NvidiaAiService : INvidiaAiService
{
    private readonly HttpClient _httpClient;
    private readonly NvidiaNimOptions _options;

    public NvidiaAiService(HttpClient httpClient, IOptions<NvidiaNimOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public Task<string> GenerateChatAsync(string prompt, string? model = null)
    {
        var messages = new[] { new ChatMessage("user", prompt) };
        return GenerateChatAsync(messages, model);
    }

    public async Task<string> GenerateChatAsync(IEnumerable<ChatMessage> messages, string? model = null)
    {
        var requestMessages = messages.Select(m => new { role = m.Role, content = m.Content }).ToArray();

        var requestBody = new
        {
            model = model ?? _options.DefaultModel,
            messages = requestMessages,
            temperature = 0.5,
            max_tokens = 1024,
            stream = false
        };

        string jsonPayload = JsonSerializer.Serialize(requestBody);

        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        content.Headers.ContentType!.CharSet = string.Empty;

        var response = await _httpClient.PostAsync("chat/completions", content);
        string responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"NVIDIA API error: {(int)response.StatusCode} - {responseString}");
        }

        using JsonDocument doc = JsonDocument.Parse(responseString);
        // Try to extract assistant content in a few common shapes
        try
        {
            string generatedText = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            return generatedText;
        }
        catch
        {
            return responseString;
        }
    }
}

public class NvidiaNimOptions
{
    public const string SectionName = "NvidiaNim";
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://integrate.api.nvidia.com/v1/";
    public string DefaultModel { get; set; } = "meta/llama-3.1-8b-instruct";
}

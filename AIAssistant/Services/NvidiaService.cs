using System.Net.Http.Headers;

namespace AIAssistant.Api.Services
{
    public class NvidiaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private const string BaseUrl = "https://build.nvidia.com";

        public NvidiaService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        /// <summary>
        /// Tests the provided (or configured) NVIDIA API key by calling the /models endpoint.
        /// Returns a tuple of (success, statusCode, content).
        /// </summary>
        public async Task<(bool Success, int StatusCode, string Content)> TestApiKeyAsync(string? apiKey = null)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = _configuration["Nvidia:ApiKey"];
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Nvidia API key is not configured.");
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/models");
            // Preferred: Authorization: Bearer <key>
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            try
            {
                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return (response.IsSuccessStatusCode, (int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                return (false, 0, ex.Message);
            }
        }
    }
}

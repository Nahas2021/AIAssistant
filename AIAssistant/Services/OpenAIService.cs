#pragma warning disable OPENAI001

using OpenAI.Responses;

namespace AIAssistant.Api.Services
{
    public class OpenAIService
    {
        private readonly ResponsesClient _client;

        public OpenAIService(IConfiguration configuration)
        {
            var apiKey = configuration["OpenAI:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException(
                    "OpenAI API key is not configured.");
            }

            _client = new ResponsesClient(apiKey);
        }

        public async Task<string> GetResponseAsync(string message)
        {
            var response = await _client.CreateResponseAsync(
                "gpt-5",
                message);

            return response.Value.GetOutputText();
        }
    }
}

#pragma warning restore OPENAI001
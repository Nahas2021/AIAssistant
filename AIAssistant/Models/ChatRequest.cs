namespace AIAssistant.Api.Models
{
    public class ChatRequest
    {
        public string ConversationId { get; set; } = string.Empty;

        private string _prompt = string.Empty;

        public string Prompt
        {
            get => _prompt;
            set => _prompt = value;
        }

        // Backwards-compatible alias used by older controllers
        public string Message
        {
            get => _prompt;
            set => _prompt = value;
        }

        public string Model { get; set; } =
            "meta/llama-3.1-8b-instruct";
    }
}
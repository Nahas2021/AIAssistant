using AIAssistant.Api.Models;
using OpenAI.Chat;

namespace AIAssistant.Api.Services;

public class ConversationService
{
    private readonly Dictionary<string, List<NvidiaChatMessage>> _conversations = new();

    public List<NvidiaChatMessage> GetHistory(string conversationId)
    {
        if (!_conversations.ContainsKey(conversationId))
        {
            _conversations[conversationId] = new List<NvidiaChatMessage>();
        }

        return _conversations[conversationId];
    }

    public void AddMessage(
        string conversationId,
        string role,
        string content)
    {
        var history = GetHistory(conversationId);

        history.Add(new NvidiaChatMessage
        {
            Role = role,
            Content = content
        });
    }
}
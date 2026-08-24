namespace AIAssistant.Data.Models;

public class Message
{
    public int Id { get; set; }

    public string ConversationId { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty; // "user" or "assistant"

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public int ConversationForeignKeyId { get; set; }

    // Navigation property
    public Conversation? Conversation { get; set; }
}

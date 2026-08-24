namespace AIAssistant.Data.Models;

public class Conversation
{
    public int Id { get; set; }

    public string ConversationId { get; set; } = Guid.NewGuid().ToString();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

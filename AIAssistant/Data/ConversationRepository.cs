using AIAssistant.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AIAssistant.Data;

public interface IConversationRepository
{
    Task<string> CreateConversationAsync();
    Task<bool> ConversationExistsAsync(string conversationId);
    Task AddMessageAsync(string conversationId, string role, string content);
    Task<List<Message>> GetMessagesAsync(string conversationId);
    Task DeleteConversationAsync(string conversationId);
}

public class ConversationRepository : IConversationRepository
{
    private readonly AppDbContext _dbContext;

    public ConversationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> CreateConversationAsync()
    {
        var conversation = new Conversation
        {
            ConversationId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Conversations.Add(conversation);
        await _dbContext.SaveChangesAsync();

        return conversation.ConversationId;
    }

    public async Task<bool> ConversationExistsAsync(string conversationId)
    {
        return await _dbContext.Conversations
            .AnyAsync(c => c.ConversationId == conversationId);
    }

    public async Task AddMessageAsync(string conversationId, string role, string content)
    {
        var conversation = await _dbContext.Conversations
            .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

        if (conversation == null)
            throw new InvalidOperationException($"Conversation '{conversationId}' not found.");

        var message = new Message
        {
            ConversationId = conversationId,
            Role = role,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            ConversationForeignKeyId = conversation.Id
        };

        _dbContext.Messages.Add(message);
        conversation.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Message>> GetMessagesAsync(string conversationId)
    {
        return await _dbContext.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task DeleteConversationAsync(string conversationId)
    {
        var conversation = await _dbContext.Conversations
            .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

        if (conversation != null)
        {
            _dbContext.Conversations.Remove(conversation);
            await _dbContext.SaveChangesAsync();
        }
    }
}

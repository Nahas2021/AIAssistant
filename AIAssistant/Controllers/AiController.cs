using Microsoft.AspNetCore.Mvc;
using AIAssistant.Api.Services;
using AIAssistant.Data;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly INvidiaAiService _aiService;
    private readonly IConversationRepository _conversationRepository;

    public ChatController(INvidiaAiService aiService, IConversationRepository conversationRepository)
    {
        _aiService = aiService;
        _conversationRepository = conversationRepository;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartConversation()
    {
        try
        {
            var conversationId = await _conversationRepository.CreateConversationAsync();
            return Ok(new { conversationId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("{conversationId}/message")]
    public async Task<IActionResult> SendMessage(string conversationId, [FromBody] SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            return BadRequest("Prompt cannot be empty.");

        try
        {
            // Check if conversation exists
            if (!await _conversationRepository.ConversationExistsAsync(conversationId))
                return NotFound("Conversation not found.");

            // Add user message to database
            await _conversationRepository.AddMessageAsync(conversationId, "user", request.Prompt);

            // Get conversation history from database
            var history = await _conversationRepository.GetMessagesAsync(conversationId);
            var messages = history.Select(m => new ChatMessage(m.Role, m.Content));

            // Call Nvidia LLM with full history
            string assistantReply = await _aiService.GenerateChatAsync(messages, request.Model);

            // Add assistant response to database
            await _conversationRepository.AddMessageAsync(conversationId, "assistant", assistantReply);

            return Ok(new { reply = assistantReply });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{conversationId}/history")]
    public async Task<IActionResult> GetHistory(string conversationId)
    {
        try
        {
            if (!await _conversationRepository.ConversationExistsAsync(conversationId))
                return NotFound("Conversation not found.");

            var messages = await _conversationRepository.GetMessagesAsync(conversationId);
            var result = messages.Select(m => new { m.Role, m.Content }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("{conversationId}")]
    public async Task<IActionResult> DeleteConversation(string conversationId)
    {
        try
        {
            if (!await _conversationRepository.ConversationExistsAsync(conversationId))
                return NotFound("Conversation not found.");

            await _conversationRepository.DeleteConversationAsync(conversationId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class SendMessageRequest
{
    public string Prompt { get; set; } = string.Empty;
    public string? Model { get; set; }
}


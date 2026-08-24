using AIAssistant.Api.Services;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AIAssistant.Api.Models;


namespace AIAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiChatController : ControllerBase
    {
        private readonly INvidiaAiService     _aiService;
        private readonly ConversationService _conversationService;

        public AiChatController(
            INvidiaAiService aiService,
            ConversationService conversationService)        
        {
            _aiService = aiService;
            _conversationService = conversationService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat(
     [FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ConversationId))
            {
                return BadRequest("ConversationId is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest("Prompt is required.");
            }

            // Get existing conversation
            var history = _conversationService
                .GetHistory(request.ConversationId);

            // Add user's new message
            _conversationService.AddMessage(
                request.ConversationId,
                "user",
                request.Prompt);

            // Send entire conversation to NVIDIA
            var messages = history.Select(h => new ChatMessage(h.Role, h.Content));
            var response = await _aiService.GenerateChatAsync(
                messages,
                request.Model);

            // Store AI response
            _conversationService.AddMessage(
                request.ConversationId,
                "assistant",
                response);

            return Ok(new
            {
                conversationId = request.ConversationId,
                response
            });
        }
    }
}

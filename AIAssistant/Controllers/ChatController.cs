using AIAssistant.Api.Models;
using AIAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly OpenAIService _openAIService;

        public ChatController(OpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat(
            [FromBody] ChatRequest request)
        {
            var response = await _openAIService
                .GetResponseAsync(request.Message);

            return Ok(new
            {
                response = response
            });
        }
    }
}
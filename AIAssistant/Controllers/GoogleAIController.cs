using AIAssistant.Api.Models;
using AIAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleAIController : ControllerBase
    {
        private readonly GoogleAIService _googleAIService;

        public GoogleAIController(GoogleAIService googleAIService)
        {
            _googleAIService = googleAIService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat(
            [FromBody] ChatRequest request)
        {
            var response = await _googleAIService
                .GetResponseAsync(request.Message);

            return Ok(new
            {
                response = response
            });
        }
    }
}

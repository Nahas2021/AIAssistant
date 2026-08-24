using AIAssistant.Api.Models;
using AIAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeepSeekController : ControllerBase
    {
        private readonly DeepSeekService _deepSeekService;

        public DeepSeekController(DeepSeekService deepSeekService)
        {
            _deepSeekService = deepSeekService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat(
            [FromBody] ChatRequest request)
        {
            var response = await _deepSeekService
                .GetResponseAsync(request.Message);

            return Ok(new
            {
                response = response
            });
        }
    }
}

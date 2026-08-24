using AIAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NvidiaController : ControllerBase
    {
        private readonly NvidiaService _nvidiaService;

        public NvidiaController(NvidiaService nvidiaService)
        {
            _nvidiaService = nvidiaService;
        }

        public class ValidateRequest
        {
            public string? ApiKey { get; set; }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] ValidateRequest? request)
        {
            try
            {
                var (success, statusCode, content) = await _nvidiaService.TestApiKeyAsync(request?.ApiKey);
                if (success)
                {
                    return Ok(new { valid = true, statusCode, content });
                }

                if (statusCode == 401 || statusCode == 403)
                {
                    return Unauthorized(new { valid = false, statusCode, content });
                }

                return BadRequest(new { valid = false, statusCode, content });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

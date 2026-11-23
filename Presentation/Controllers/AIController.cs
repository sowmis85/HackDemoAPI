using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace HackDemoAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/events/ai")]
    public class AiController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AiController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("describe")]
        public async Task<IActionResult> GenerateDescription([FromBody] AIEventDescRequest request, CancellationToken cancellationToken)
        {
            var description = await _aiService.GenerateEventAIDescriptionAsync(request, cancellationToken);

            return Ok(new AIEventDescResponse
            {
                GeneratedAIDescription = description
            });
        }
    }
}

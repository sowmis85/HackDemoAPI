using HackDemoAPI.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackDemoAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/registrations/workflows")]
    public class RegWorkflowController : ControllerBase
    {
        private readonly IWorkflowService _workflowService;

        public RegWorkflowController(IWorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        [HttpGet("{instanceId}")]
        public async Task<IActionResult> GetWorkflowStatus(string instanceId, CancellationToken cancellationToken)
        {
            var response =
                await _workflowService.GetStatusAsync(
                    instanceId,
                    cancellationToken);

            return Ok(response);
        }
    }
}

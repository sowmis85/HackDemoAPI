using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace HackDemoAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/registrations")]
    public class RegistrationsController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationsController(
            IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost("{eventId}")]
        public async Task<IActionResult> Register(string eventId, [FromBody] EventRegistrationRequest request,
            CancellationToken cancellationToken)
        {
            var response =
                await _registrationService.RegisterAsync(
                    eventId,
                    request,
                    cancellationToken);

            return Ok(response);
        }

        [HttpGet("{registrationId}")]
        public async Task<IActionResult> GetRegistration(string registrationId, CancellationToken cancellationToken)
        {
            var response =
                await _registrationService.GetByIdAsync(
                    registrationId,
                    cancellationToken);

            return Ok(response);
        }

        [HttpDelete("{registrationId}")]
        public async Task<IActionResult> CancelRegistration(string registrationId, CancellationToken cancellationToken)
        {
            var response =
                await _registrationService.CancelEventRegisterAsync(
                    registrationId,
                    cancellationToken);

            return Ok(response);
        }
    }
}

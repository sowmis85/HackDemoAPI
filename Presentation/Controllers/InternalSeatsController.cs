using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.Interfaces.IService;
using Microsoft.AspNetCore.Mvc;

namespace HackDemoAPI.Presentation.Controllers
{
    [ApiController]
    [Route("internal/events")]
    public class InternalSeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public InternalSeatsController(
            ISeatService seatService)
        {
            _seatService = seatService;
        }

        [HttpPost("{eventId}/seats/reserve")]
        public async Task<IActionResult> ReserveSeat(string eventId, [FromBody] SeatRequest request, CancellationToken cancellationToken)
        {
            var idempotencyKey =
                Request.Headers["Idempotency-Key"]
                       .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                return BadRequest(
                    "Idempotency-Key header required");
            }

            var response = await _seatService.ReserveAsync(eventId, idempotencyKey, request, cancellationToken);

            return Ok(response);
        }

        [HttpPost("{eventId}/seats/release")]
        public async Task<IActionResult> ReleaseSeat(string eventId, [FromBody] SeatRequest request, CancellationToken cancellationToken)
        {
            var idempotencyKey =
                Request.Headers["Idempotency-Key"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                return BadRequest(
                    "Idempotency-Key header required");
            }

            var response = await _seatService.ReleaseAsync(eventId, idempotencyKey, request, cancellationToken);

            return Ok(response);
        }
    }
}

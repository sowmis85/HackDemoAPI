using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace HackDemoAPI.Presentation.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents(
            [FromQuery] EventSearchRequest request, CancellationToken cancellationToken)
        {
            var result = await _eventService.GetEventsAsync(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{eventId}")]
        [ProducesResponseType(typeof(EventDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEventById(string eventId, CancellationToken cancellationToken)
        {
            var eventData = await _eventService.GetEventByIdAsync(eventId, cancellationToken);

            if (eventData == null)
            {
                return NotFound(new
                {
                    Message = $"Event '{eventId}' not found."
                });
            }

            return Ok(eventData);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Create(CRUDEventRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value
                            ?? User.FindFirst("email")?.Value
                            ?? "Unknown";

                var response = await _eventService.CreateAsync(request, email, cancellationToken);
                return Created($"/api/events/{response.EventId}", response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { error = "An unexpected error occurred." });
            }
        }

        [HttpPut("{eventId}")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Update(string eventId, [FromBody] CRUDEventRequest request, CancellationToken cancellationToken)
        {
            var response = await _eventService.UpdateAsync(eventId, request, cancellationToken);

            return Ok(response);
        }

        [HttpDelete("{eventId}")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Delete(string eventId, CancellationToken cancellationToken)
        {
            var response = await _eventService.DeleteAsync(eventId, cancellationToken);

            return Ok(response);
        }

        [HttpPut("{eventId}/publish")]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> PublishEvent(string eventId, CancellationToken cancellationToken)
        {
            var response= await _eventService.PublishEventAsync(eventId, cancellationToken);

            return Ok(response);
        }
    }
}

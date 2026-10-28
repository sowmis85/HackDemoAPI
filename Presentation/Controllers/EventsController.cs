using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    }
}

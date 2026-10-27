using HackDemoAPI.Application.DTO.Requests;
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
    }
}

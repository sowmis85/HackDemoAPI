using HackDemoAPI.Application.DTO.Events;
using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;

namespace HackDemoAPI.Application.Interfaces
{
    public interface IEventService
    {
        Task<EventListResponse> GetEventsAsync(EventSearchRequest request, CancellationToken cancellationToken);

        Task<EventDetailsResponse?> GetEventByIdAsync(string eventId, CancellationToken cancellationToken);
    }
}

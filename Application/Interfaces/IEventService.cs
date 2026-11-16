using HackDemoAPI.Application.DTO.Events;
using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;

namespace HackDemoAPI.Application.Interfaces
{
    public interface IEventService
    {
        Task<EventListResponse> GetEventsAsync(EventSearchRequest request, CancellationToken cancellationToken);

        Task<EventDetailsResponse?> GetEventByIdAsync(string eventId, CancellationToken cancellationToken);

        Task<CRUDEventResponse> CreateAsync(CRUDEventRequest request, string createdBy, CancellationToken cancellationToken);

        Task<CRUDEventResponse> UpdateAsync(string eventId, CRUDEventRequest request, CancellationToken cancellationToken);

        Task<CRUDEventResponse> DeleteAsync(string eventId, CancellationToken cancellationToken);
    }
}

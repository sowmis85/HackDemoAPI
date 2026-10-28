using HackDemoAPI.Application.DTO.Events;
using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Domain.Entities;

namespace HackDemoAPI.Application.Interfaces
{
    public interface IEventRepository
    {
        Task<EventListResponse> GetEventsAsync(EventSearchRequest request, CancellationToken cancellationToken);

        Task<Event?> GetByIdAsync(string eventId, CancellationToken cancellationToken);
    }
}

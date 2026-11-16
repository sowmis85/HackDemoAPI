using HackDemoAPI.Application.DTO.Events;
using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Domain.Entities;

namespace HackDemoAPI.Application.Interfaces
{
    public interface IEventRepository
    {
        Task<EventListResponse> GetEventsAsync(EventSearchRequest request, CancellationToken cancellationToken);

        Task<Event?> GetByIdAsync(string eventId, CancellationToken cancellationToken);

        Task CreateEventAsync(Event eventItem, CancellationToken cancellationToken);

        Task<List<Event>> GetAllAsync(CancellationToken cancellationToken);

        Task UpdateAsync(Event eventItem, CancellationToken cancellationToken);

        Task DeleteAsync(string eventId, CancellationToken cancellationToken);
    }
}

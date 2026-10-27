using HackDemoAPI.Application.DTO.Events;
using HackDemoAPI.Application.DTO.Requests;

namespace HackDemoAPI.Application.Interfaces
{
    public interface IEventRepository
    {
        Task<EventListResponse> GetEventsAsync(EventSearchRequest request, CancellationToken cancellationToken);
    }
}

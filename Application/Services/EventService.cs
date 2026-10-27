using HackDemoAPI.Application.DTO.Events;
using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.Interfaces;
using System.Threading;

namespace HackDemoAPI.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repository;

        public EventService(IEventRepository repository)
        {
            _repository = repository;
        }

        public async Task<EventListResponse> GetEventsAsync(
            EventSearchRequest request, CancellationToken cancellationToken)
        {
            return await _repository.GetEventsAsync(request, cancellationToken);
        }
    }
}

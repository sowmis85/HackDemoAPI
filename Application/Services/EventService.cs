using HackDemoAPI.Application.DTO.Events;
using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Interfaces;
using HackDemoAPI.Infrastructure;
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

        public async Task<EventListResponse> GetEventsAsync(EventSearchRequest request, CancellationToken cancellationToken)
        {
            return await _repository.GetEventsAsync(request, cancellationToken);
        }

        public async Task<EventDetailsResponse?> GetEventByIdAsync(string eventId, CancellationToken cancellationToken)
        {
            var eventData = await _repository.GetByIdAsync(eventId, cancellationToken);

            if (eventData == null)
                return null;

            return new EventDetailsResponse
            {
                EventId = eventData.EventId,
                Title = eventData.Title,
                Description = eventData.Description,
                Category = eventData.Category.ToString(),                
                Capacity = eventData.Capacity,   
                StartDate = eventData.StartDate,
                EndDate = eventData.EndDate,
                ReservedSeats=eventData.ReservedSeats,
                AvailableSeats =eventData.Capacity-eventData.ReservedSeats                   
            };
        }
    }
}

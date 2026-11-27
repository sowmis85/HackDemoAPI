using HackDemoAPI.Application.DTO.Events;
using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Interfaces.IRepository;
using HackDemoAPI.Application.Interfaces.IService;
using HackDemoAPI.Common;
using HackDemoAPI.Domain.Entities;
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
                Location=eventData.Location,
                StartDate = eventData.StartDate,
                EndDate = eventData.EndDate,
                Status=eventData.Status,
                Capacity = eventData.Capacity,
                ReservedSeats =eventData.ReservedSeats,
                AvailableSeats =eventData.Capacity-eventData.ReservedSeats                   
            };
        }

        public async Task<CRUDEventResponse> CreateAsync(CRUDEventRequest request, string createdBy, CancellationToken cancellationToken)
        {
            var events = await _repository.GetAllAsync(cancellationToken);

            var nextNumber = events.Any()
                             ? events.Max(e => int.Parse(e.EventId[3..])) + 1
                             : 1001;

            var eventId = $"EVT{nextNumber}";

            var newEvent = new Event
            {
                EventId = eventId,
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                Location=request.Location,
                StartDate = request.StartDate,  
                EndDate=request.EndDate,
                Capacity = request.Capacity,
                AvailableSeats = request.Capacity,
                Status=EventStatus.Draft.ToString(),
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow
            };

            await _repository.CreateEventAsync(newEvent, cancellationToken);

            return new CRUDEventResponse
            {
                EventId = eventId,
                Message = ValidationConstant.EventCreated ?? string.Empty
            };
        }

        public async Task<CRUDEventResponse> UpdateAsync(string eventId, CRUDEventRequest request, CancellationToken cancellationToken)
        {
            var existingEvent = await _repository.GetByIdAsync(eventId, cancellationToken);

            if (existingEvent is null)
            {
                throw new KeyNotFoundException($"Event '{eventId}' not found.");
            }

            existingEvent.Title = request.Title;
            existingEvent.Description = request.Description;
            existingEvent.Category = request.Category;
            existingEvent.StartDate = request.StartDate;
            existingEvent.EndDate = request.EndDate;

            var bookedSeats = existingEvent.Capacity - existingEvent.AvailableSeats;
            existingEvent.Capacity = request.Capacity;
            existingEvent.AvailableSeats = request.Capacity - bookedSeats;
           
            await _repository.UpdateAsync(existingEvent, cancellationToken);

            return new CRUDEventResponse
            {
                EventId = eventId,
                Message = ValidationConstant.EventUpdated ?? string.Empty
            };
        }

        public async Task<CRUDEventResponse> DeleteAsync(string eventId, CancellationToken cancellationToken)
        {
            var existingEvent = await _repository.GetByIdAsync(eventId, cancellationToken);

            if (existingEvent is null)
            {
                throw new KeyNotFoundException(
                    $"Event '{eventId}' not found.");
            }

            await _repository.DeleteAsync(eventId, cancellationToken);

            return new CRUDEventResponse
            {
                EventId = eventId,
                Message = ValidationConstant.EventDeleted ?? string.Empty
            };
        }

        public async Task<CRUDEventResponse> PublishEventAsync(string eventId, CancellationToken cancellationToken)
        {          
            await _repository.PublishEventAsync(eventId, cancellationToken);

            return new CRUDEventResponse
            {
                EventId = eventId,
                Message = ValidationConstant.Published ?? string.Empty
            };
        }
    }
}

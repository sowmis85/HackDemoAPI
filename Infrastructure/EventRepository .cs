namespace HackDemoAPI.Infrastructure
{
    using Application.Interfaces;
    using Domain.Entities;
    using HackDemoAPI.Application.DTO.Events;
    using HackDemoAPI.Application.DTO.Requests;
    using Microsoft.Extensions.Logging;
    using System.Threading;

    public class EventRepository : IEventRepository
    {       
        private readonly string _filePath;
        private readonly IJsonFileHelper _jsonFileHelper;

        public EventRepository(IWebHostEnvironment environment, IJsonFileHelper jsonFileHelper)
        {
            _filePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "events.json");
            _jsonFileHelper = jsonFileHelper;
        }

        public async Task<EventListResponse> GetEventsAsync(
            EventSearchRequest request, CancellationToken cancellationToken)
        {
            var events = await _jsonFileHelper.ReadAsync<Event>(_filePath, cancellationToken);

            IEnumerable<Event> query = events;

            // Search
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(x =>
                    (!string.IsNullOrEmpty(x.Title) &&
                     x.Title.Contains(request.Search, StringComparison.OrdinalIgnoreCase))
                    ||
                    (!string.IsNullOrEmpty(x.Description) &&
                     x.Description.Contains(request.Search, StringComparison.OrdinalIgnoreCase)));                   
            } 

            // Date filter
            if (request.StartDate.HasValue)
            {
                query = query.Where(x =>
                    x.StartDate >= request.StartDate.Value.Date);
            }

            if (request.EndDate.HasValue)
            {
               // var endDate = request.EndDate.Value.Date.AddDays(1);
                query = query.Where(x => x.EndDate.Date <= request.EndDate.Value.Date);
            }

            // Sorting
            query = request.SortBy?.ToLower() switch
            {
                "name" => request.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(x => x.Title)
                    : query.OrderBy(x => x.Title),

                "starteddate" => request.SortOrder.ToLower() == "desc"
                    ? query.OrderByDescending(x => x.StartDate)
                    : query.OrderBy(x => x.StartDate),

                _ => query // or handle as needed
            };

            var totalRecords = query.Count();

            var eventDtos = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new Event
                {
                    EventId = x.EventId,
                    Title = x.Title,
                    Description = x.Description,
                    Category = x.Category,
                    StartDate = x.StartDate,
                    Capacity = x.Capacity,
                    EndDate=x.EndDate,
                    ReservedSeats = x.ReservedSeats,
                    Status = x.Status,
                    AvailableSeats= x.AvailableSeats,
                    OrganizerId = x.OrganizerId
                })
                .ToList();

            return new EventListResponse
            {
                Events = eventDtos,
                TotalRecords = totalRecords,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(
                    totalRecords / (double)request.PageSize)
            };
        }

        public async Task<Event?> GetByIdAsync(string eventId, CancellationToken cancellationToken)
        {

            var events = await _jsonFileHelper.ReadAsync<Event>(_filePath, cancellationToken);

            return events.FirstOrDefault(x =>
                x.EventId.Equals(eventId,
                    StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<Event>> GetAllAsync(CancellationToken cancellationToken)
        {
            var events = await _jsonFileHelper.ReadAsync<Event>(_filePath, cancellationToken);

            return events.ToList();
        }

        public async Task CreateEventAsync(Event eventItem, CancellationToken cancellationToken)
        {
            var events = await _jsonFileHelper.ReadAsync<Event>(_filePath, cancellationToken);

            events.Add(eventItem);

            await _jsonFileHelper.WriteAsync(
                _filePath,
                events,
                cancellationToken);
        }
        public async Task UpdateAsync(Event eventItem, CancellationToken cancellationToken)
        {
            var events = await _jsonFileHelper.ReadAsync<Event>(_filePath, cancellationToken);

            var existingEvent = events.FirstOrDefault(x =>
                x.EventId == eventItem.EventId);

            if (existingEvent is null)
            {
                throw new KeyNotFoundException(
                    "Event not found");
            }

            existingEvent.Title = eventItem.Title;
            existingEvent.Description = eventItem.Description;
            existingEvent.Category = eventItem.Category;
            existingEvent.StartDate = eventItem.StartDate;
            existingEvent.EndDate = eventItem.EndDate;
            existingEvent.Capacity = eventItem.Capacity;
            existingEvent.AvailableSeats = eventItem.AvailableSeats;

            await _jsonFileHelper.WriteAsync(
                _filePath,
                events,
                cancellationToken);
        }
        public async Task DeleteAsync(string eventId, CancellationToken cancellationToken)
        {
            var events = await _jsonFileHelper.ReadAsync<Event>(_filePath, cancellationToken);

            var existingEvent = events.FirstOrDefault(x => x.EventId.Equals(eventId,
                    StringComparison.OrdinalIgnoreCase));

            if (existingEvent is null)
            {
                throw new KeyNotFoundException(
                    $"Event '{eventId}' not found.");
            }

            events.Remove(existingEvent);

            await _jsonFileHelper.WriteAsync(
                _filePath,
                events,
                cancellationToken);
        }
    }
}

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
                    x.StartDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(x =>
                    x.EndDate <= request.EndDate.Value);
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
                    Capacity = x.Capacity                   
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
    }
}

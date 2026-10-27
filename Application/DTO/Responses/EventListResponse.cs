using HackDemoAPI.Domain.Entities;

namespace HackDemoAPI.Application.DTO.Events
{
    public class EventListResponse
    {
        public required List<Event> Events { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; } = 0;
    }
}

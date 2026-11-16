namespace HackDemoAPI.Application.DTO.Responses
{
    public class EventDetailsResponse
    {
        public string EventId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int ReservedSeats { get; set; }
        public int AvailableSeats { get; set; }        
    }
}

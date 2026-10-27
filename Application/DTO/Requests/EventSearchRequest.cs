namespace HackDemoAPI.Application.DTO.Requests
{
    public class EventSearchRequest
    {
        public string? Search { get; set; }
        public string? Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string SortBy { get; set; } = "EventDate";
        public string SortOrder { get; set; } = "asc";

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

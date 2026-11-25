namespace HackDemoAPI.Application.DTO.Responses
{
    public class UserEventRegisResponse
    {
        public string RegistrationId { get; set; } = string.Empty;

        public string EventId { get; set; } = string.Empty;

        public string EventName { get; set; } = string.Empty;

        public string AttendeeName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}

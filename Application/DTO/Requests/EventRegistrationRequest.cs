namespace HackDemoAPI.Application.DTO.Requests
{
    public class EventRegistrationRequest
    {
        public string AttendeeName { get; set; } = string.Empty;

        public string AttendeeEmail { get; set; } = string.Empty;

        public int Quantity { get; set; } = 1;

        public string CreatedBy { get; set; } = string.Empty;
    }
}

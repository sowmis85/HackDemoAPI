namespace HackDemoAPI.Domain.Entities
{
    public class Registration
    {
        public string RegistrationId { get; set; } = string.Empty;

        public string EventId { get; set; } = string.Empty;

        public string AttendeeName { get; set; } = string.Empty;

        public string AttendeeEmail { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string WorkflowInstanceId { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
    }
}

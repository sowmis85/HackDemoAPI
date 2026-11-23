namespace HackDemoAPI.Application.DTO.Responses
{
    public class EventRegistrationResponse
    {
        public string RegistrationId { get; set; } = string.Empty;

        public string EventId { get; set; } = string.Empty;

        public string AttendeeName { get; set; } = string.Empty;

        public string AttendeeEmail { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string WorkflowInstanceId { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }

        public string InstanceStatusUri { get; set; } = string.Empty;

        public string InstanceTerminateUri { get; set; } = string.Empty;

    }
}

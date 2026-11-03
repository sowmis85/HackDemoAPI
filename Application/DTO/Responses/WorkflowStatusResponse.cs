namespace HackDemoAPI.Application.DTO.Responses
{
    public class WorkflowStatusResponse
    {
        public string WorkflowInstanceId { get; set; } = string.Empty;

        public string RegistrationId { get; set; } = string.Empty;

        public string CurrentState { get; set; } = string.Empty;

        public DateTime StartedOn { get; set; }

        public DateTime LastUpdatedOn { get; set; }
    }
}

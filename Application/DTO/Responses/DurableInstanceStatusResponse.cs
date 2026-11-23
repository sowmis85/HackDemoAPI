namespace HackDemoAPI.Application.DTO.Responses
{
    public class DurableInstanceStatusResponse
    {
        public string InstanceId { get; set; } = string.Empty;

        public string StatusQueryGetUri { get; set; } = string.Empty;

        public string SendEventPostUri { get; set; } = string.Empty;

        public string TerminatePostUri { get; set; } = string.Empty;

        public string PurgeHistoryDeleteUri { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}

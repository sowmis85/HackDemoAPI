namespace HackDemoAPI.Domain.Entities
{
    public static class WorkflowState
    {
        public const string Started = "Started";

        public const string SeatReserved = "SeatReserved";

        public const string RegistrationCreated = "RegistrationCreated";

        public const string Completed = "Completed";

        public const string Failed = "Failed";

        public const string Cancelled = "Cancelled";
    }
}

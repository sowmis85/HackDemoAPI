namespace HackDemoAPI.Application.DTO.Responses
{
    public class IdempotencyRecord
    {
        public string IdempotencyKey { get; set; } = string.Empty;

        public string EventId { get; set; } = string.Empty;

        public string Operation { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }
    }
}

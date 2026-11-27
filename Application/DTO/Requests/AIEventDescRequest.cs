namespace HackDemoAPI.Application.DTO.Requests
{
    public class AIEventDescRequest
    {
        public string Title { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string TargetAudience { get; set; } = string.Empty;

        public List<string> Keywords { get; set; } = new();
    }
}

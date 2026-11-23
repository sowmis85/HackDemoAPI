namespace HackDemoAPI.Application.DTO.Responses
{
    public sealed class AuthResponse
    {
        public Guid UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
        public string? ErrorCode { get; set; }
    }
}

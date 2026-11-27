namespace HackDemoAPI.Application.DTO.Responses
{
    public sealed class LoginResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? Role { get; set; }
        public string? UserId { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
    }
}

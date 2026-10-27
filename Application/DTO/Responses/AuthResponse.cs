namespace HackDemoAPI.Application.DTO.Responses
{
    public sealed class AuthResponse
    {
        public Guid UserId { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}

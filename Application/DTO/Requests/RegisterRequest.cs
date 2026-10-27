namespace HackDemoAPI.Application.DTO.Requests
{
    public sealed class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Role { get; set; } = string.Empty;
    }
}

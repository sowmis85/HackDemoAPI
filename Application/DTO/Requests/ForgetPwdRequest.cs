namespace HackDemoAPI.Application.DTO.Requests
{
    public class ForgetPwdRequest
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}

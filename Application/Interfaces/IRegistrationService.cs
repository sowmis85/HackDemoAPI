using HackDemoAPI.Application.DTO;

namespace HackDemoAPI.Application.Interfaces
{
    public interface IRegistrationService
    {
        Task<object> RegisterUserAsync(LoginRequest request);
    }
}

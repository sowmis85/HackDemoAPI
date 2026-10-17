using HackDemoAPI.DTO;

namespace HackDemoAPI.Services
{
    public interface IRegistrationService
    {
        Task<object> RegisterUserAsync(RegistrationDTO request);
    }
}

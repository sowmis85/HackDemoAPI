using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;

namespace HackDemoAPI.Application.Interfaces.IService
{
    public interface IRegistrationService
    {
        Task<EventRegistrationResponse> RegisterAsync(string eventId, EventRegistrationRequest request, CancellationToken cancellationToken);

        Task<EventRegistrationResponse> GetByIdAsync(string registrationId, CancellationToken cancellationToken);

        Task<CancelRegistrationResponse> CancelEventRegisterAsync(string registrationId, CancellationToken cancellationToken);

        Task<IEnumerable<UserEventRegisResponse>> GetRegistrationsByUserIdAsync(string userId, CancellationToken cancellationToken);
    }
}

using HackDemoAPI.Application.DTO.Responses;

namespace HackDemoAPI.Application.Interfaces
{
    public interface IDurableFunctionClient
    {
        Task<DurableInstanceStatusResponse> StartRegistrationOrchestratorAsync(
         string eventId,
         string userId,
         string registrationId,
         CancellationToken cancellationToken);
    }
}

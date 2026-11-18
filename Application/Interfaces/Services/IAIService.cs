using HackDemoAPI.Application.DTO.Requests;

namespace HackDemoAPI.Application.Interfaces.IService
{
    public interface IAIService
    {
        Task<string> GenerateEventAIDescriptionAsync(AIEventDescRequest request, CancellationToken cancellationToken);
    }
}

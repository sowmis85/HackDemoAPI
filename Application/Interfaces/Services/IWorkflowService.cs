using HackDemoAPI.Application.DTO.Responses;

namespace HackDemoAPI.Application.Interfaces.Services
{
    public interface IWorkflowService
    {
        Task<WorkflowStatusResponse> GetStatusAsync(string instanceId, CancellationToken cancellationToken);
    }
}

using HackDemoAPI.Domain.Entities;

namespace HackDemoAPI.Application.Interfaces.Repositories
{
    public interface IWorkflowRepository
    {
        Task<WorkflowInstance?> GetByInstanceIdAsync(string instanceId, CancellationToken cancellationToken);
        Task CreateAsync(WorkflowInstance workflowInstance, CancellationToken cancellationToken);
    }
}

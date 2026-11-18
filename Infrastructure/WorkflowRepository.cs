using HackDemoAPI.Application.Interfaces;
using HackDemoAPI.Application.Interfaces.Repositories;
using HackDemoAPI.Domain.Entities;

namespace HackDemoAPI.Infrastructure
{
    public class WorkflowRepository : IWorkflowRepository
    {
        private readonly IJsonFileHelper _jsonFileHelper;
        private readonly string _filePath;

        public WorkflowRepository(
            IWebHostEnvironment environment,
            IJsonFileHelper jsonFileHelper)
        {
            _jsonFileHelper = jsonFileHelper;

            _filePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "workflows.json");
        }

        public async Task<WorkflowInstance?> GetByInstanceIdAsync(string instanceId, CancellationToken cancellationToken)
        {
            var workflows =
                await _jsonFileHelper.ReadAsync<WorkflowInstance>(
                    _filePath,
                    cancellationToken);

            return workflows.FirstOrDefault(x =>
                x.WorkflowInstanceId.Equals(
                    instanceId,
                    StringComparison.OrdinalIgnoreCase));
        }

        public async Task CreateAsync(WorkflowInstance workflowInstance, CancellationToken cancellationToken)
        {
            var workflows =
                await _jsonFileHelper.ReadAsync<WorkflowInstance>(
                    _filePath,
                    cancellationToken);

            workflows.Add(workflowInstance);

            await _jsonFileHelper.WriteAsync(
                _filePath,
                workflows,
                cancellationToken);
        }
    }
}

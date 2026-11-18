using HackDemoAPI.Application.Exceptions;
using HackDemoAPI.Application.Interfaces;
using HackDemoAPI.Application.Interfaces.Repositories;
using HackDemoAPI.Common;
using HackDemoAPI.Domain.Entities;
using Microsoft.Extensions.Logging;

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

        public async Task UpdateAsync(WorkflowInstance workflowInstance, CancellationToken cancellationToken)
        {
            if (workflowInstance == null)
            {
                throw new ArgumentNullException(nameof(workflowInstance));
            }            

            var workflows = await _jsonFileHelper.ReadAsync<WorkflowInstance>(
                _filePath,
                cancellationToken);
           
            var existingWorkflow = workflows.FirstOrDefault(x => x.RegistrationId.Equals(workflowInstance.RegistrationId, StringComparison.OrdinalIgnoreCase));
            if (existingWorkflow is null)
            {
                throw new NotFoundException(ValidationConstant.WorkflowNotFound);
            }

            // Update the LastUpdatedOn timestamp if not already set by caller
            existingWorkflow.LastUpdatedOn = workflowInstance.LastUpdatedOn == default
                ? DateTime.UtcNow
                : workflowInstance.LastUpdatedOn;
            existingWorkflow.CurrentState = workflowInstance.CurrentState;
            existingWorkflow.RegistrationId = workflowInstance.RegistrationId;           

            await _jsonFileHelper.WriteAsync(
                _filePath,
                workflows,
                cancellationToken);
        }
    }
}

using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Exceptions;
using HackDemoAPI.Application.Interfaces.Repositories;
using HackDemoAPI.Application.Interfaces.Services;
using HackDemoAPI.Common;

namespace HackDemoAPI.Application.Services
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowRepository _workflowRepository;

        public WorkflowService(
            IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }

        public async Task<WorkflowStatusResponse> GetStatusAsync(string instanceId, CancellationToken cancellationToken)
        {
            var workflow =
                await _workflowRepository.GetByInstanceIdAsync(
                    instanceId,
                    cancellationToken);

            if (workflow == null)
            {
                throw new NotFoundException(ValidationConstant.WorkflowNotFound);
            }

            return new WorkflowStatusResponse
            {
                WorkflowInstanceId = workflow.WorkflowInstanceId,
                RegistrationId = workflow.RegistrationId,
                CurrentState = workflow.CurrentState,
                StartedOn = workflow.StartedOn,
                LastUpdatedOn = workflow.LastUpdatedOn
            };
        }
    }
}

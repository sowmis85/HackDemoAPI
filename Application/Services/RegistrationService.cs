using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Exceptions;
using HackDemoAPI.Application.Interfaces;
using HackDemoAPI.Application.Interfaces.IRepository;
using HackDemoAPI.Application.Interfaces.IService;
using HackDemoAPI.Application.Interfaces.Repositories;
using HackDemoAPI.Common;
using HackDemoAPI.Domain.Entities;
using HackDemoAPI.Infrastructure;
using System.ComponentModel.DataAnnotations;
using ValidationException = HackDemoAPI.Application.Exceptions.ValidationException;

namespace HackDemoAPI.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IWorkflowRepository _workflowRepository;
        private readonly ISeatService _seatService;
        private readonly IDurableFunctionClient _durableFunctionClient;

        public RegistrationService(
            IRegistrationRepository registrationRepository, IEventRepository eventRepository, IWorkflowRepository workflowRepository, ISeatService seatService, IDurableFunctionClient durableFunctionClient)
        {
            _registrationRepository = registrationRepository;
            _eventRepository = eventRepository;
            _seatService = seatService;
            _workflowRepository = workflowRepository;
            _durableFunctionClient = durableFunctionClient;
        }

        public async Task<EventRegistrationResponse> RegisterAsync(string eventId, EventRegistrationRequest request, CancellationToken cancellationToken)
        {
            await ValidateRegistrationAsync(eventId, request, cancellationToken);

            var registrationId =
                $"REG{Guid.NewGuid():N}"[..12];

            var workflowInstanceId =
                $"WF{Guid.NewGuid():N}"[..12];

            var idempotencyKey =
                $"{eventId}-{registrationId}-RESERVE";

            
            //registration creation
            var registration = new Registration
            {
                RegistrationId = registrationId,
                EventId = eventId,
                AttendeeName = request.AttendeeName,
                AttendeeEmail = request.AttendeeEmail,
                Quantity = request.Quantity,
                WorkflowInstanceId = workflowInstanceId,
                Status = "Started",
                CreatedOn = DateTime.UtcNow
            };

            await _registrationRepository.CreateAsync(
                registration,
                cancellationToken);

            //workflow creation
            var workflow = new WorkflowInstance
            {
                WorkflowInstanceId = workflowInstanceId,
                RegistrationId = registrationId,
                CurrentState = WorkflowState.Started,
                StartedOn = DateTime.UtcNow,
                LastUpdatedOn = DateTime.UtcNow
            };

            await _workflowRepository.CreateAsync(
                workflow,
                cancellationToken);

            // Start the registration orchestrator via HTTP starter function
            var durableResponse = await _durableFunctionClient.StartRegistrationOrchestratorAsync(
                eventId,
                request.AttendeeEmail,
                registrationId,
                cancellationToken);

            return new EventRegistrationResponse
            {
                RegistrationId = registrationId,
                WorkflowInstanceId = workflowInstanceId,
                EventId = eventId,
                AttendeeName = registration.AttendeeName,
                AttendeeEmail = registration.AttendeeEmail,
                Quantity = registration.Quantity,
                CreatedOn = registration.CreatedOn,
                Status = "In Progress",
                InstanceStatusUri = durableResponse.StatusQueryGetUri,
                InstanceTerminateUri = durableResponse.TerminatePostUri
            };
        }

        private async Task ValidateRegistrationAsync(string eventId, EventRegistrationRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                throw new ValidationException(RegValidation.EventIdRequired);
            }

            if (string.IsNullOrWhiteSpace(request.AttendeeName))
            {
                throw new ValidationException(RegValidation.AttendeeNameRequired);
            }

            if (string.IsNullOrWhiteSpace(request.AttendeeEmail))
            {
                throw new ValidationException(RegValidation.AttendeeEmailRequired);
            }

            if (!new EmailAddressAttribute()
                    .IsValid(request.AttendeeEmail))
            {
                throw new ValidationException(RegValidation.InvalidEmail);
            }

            if (request.Quantity <= 0)
            {
                throw new ValidationException(RegValidation.InvalidQuantity);
            }

            var eventEntity = await _eventRepository.GetByIdAsync(eventId, cancellationToken);

            if (eventEntity == null)
            {
                throw new NotFoundException(ValidationConstant.EventNotFound);
            }

            if (eventEntity.Status != EventStatus.Published.ToString())
            {
                throw new ValidationException(RegValidation.EventNotOpenForRegistration);
            }

            var existingRegistration =
                await _registrationRepository.GetByEmailAndEventAsync(
                    request.AttendeeEmail,
                    eventId,
                    cancellationToken);

            if (existingRegistration != null)
            {       
                throw new ValidationException(
                    RegValidation.AlreadyRegisteredForEvent);
            }
        }

        public async Task<EventRegistrationResponse> GetByIdAsync(string registrationId, CancellationToken cancellationToken)
        {
            var registration =
                await _registrationRepository.GetByIdAsync(
                    registrationId,
                    cancellationToken);

            if (registration == null)
            {
                throw new ValidationException(RegValidation.RegistrationNotFound);
            }

            return new EventRegistrationResponse
            {
                RegistrationId = registration.RegistrationId,
                EventId = registration.EventId,
                AttendeeName = registration.AttendeeName,
                AttendeeEmail = registration.AttendeeEmail,
                Quantity = registration.Quantity,
                WorkflowInstanceId = registration.WorkflowInstanceId,
                Status = registration.Status,
                CreatedOn = registration.CreatedOn
            };
        }

        public async Task<CancelRegistrationResponse> CancelEventRegisterAsync(string registrationId, CancellationToken cancellationToken)
        {
            var registration =
                await _registrationRepository.GetByIdAsync(registrationId, cancellationToken);

            if (registration == null)
            {
                throw new ValidationException(RegValidation.RegistrationNotFound);
            }

            if (registration.Status == RegistrationStatus.Cancelled.ToString())
            {
                throw new ValidationException(RegValidation.RegistrationAlreadyCancelled);
            }

            string idempotentKey = registration.RegistrationId + "-RESERVE";

            await _seatService.ReleaseAsync(
                registration.EventId,
                idempotentKey,
                new SeatRequest
                {
                    RegistrationId = registration.RegistrationId,
                    Quantity = registration.Quantity
                },
                cancellationToken);

            registration.Status = RegistrationStatus.Cancelled.ToString();

            await _registrationRepository.UpdateAsync(
                registration,
                cancellationToken);

            return new CancelRegistrationResponse
            {
                RegistrationId = registrationId,
                Status = registration.Status,
                Message = RegValidation.RegistrationCancelledSuccessfully
            };
        }
    }
}

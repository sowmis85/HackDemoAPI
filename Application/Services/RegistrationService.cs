using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Interfaces.IRepository;
using HackDemoAPI.Application.Interfaces.IService;
using HackDemoAPI.Common;
using HackDemoAPI.Domain.Entities;
using HackDemoAPI.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace HackDemoAPI.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ISeatService _seatService;

        public RegistrationService(
            IRegistrationRepository registrationRepository, IEventRepository eventRepository, ISeatService seatService)
        {
            _registrationRepository = registrationRepository;
            _eventRepository = eventRepository;
            _seatService = seatService;
        }

        public async Task<EventRegistrationResponse> RegisterAsync(string eventId, EventRegistrationRequest request, CancellationToken cancellationToken)
        {
            await ValidateRegistrationAsync(eventId, request, cancellationToken);

            var registrationId =
                $"REG{Guid.NewGuid():N}"[..12];

            var workflowInstanceId =
                $"WF{Guid.NewGuid():N}"[..12];

            var idempotencyKey =
                $"{registrationId}-RESERVE";

            await _seatService.ReserveAsync(
                eventId,
                idempotencyKey,
                new SeatRequest
                {
                    RegistrationId = registrationId,
                    Quantity = request.Quantity
                },
                cancellationToken);

            var registration = new Registration
            {
                RegistrationId = registrationId,
                EventId = eventId,
                AttendeeName = request.AttendeeName,
                AttendeeEmail = request.AttendeeEmail,
                Quantity = request.Quantity,
                WorkflowInstanceId = workflowInstanceId,
                Status = "Pending",
                CreatedOn = DateTime.UtcNow
            };

            await _registrationRepository.CreateAsync(
                registration,
                cancellationToken);

            return new EventRegistrationResponse
            {
                RegistrationId = registrationId,
                WorkflowInstanceId = workflowInstanceId,
                Status = "Pending"
            };
        }

        private async Task ValidateRegistrationAsync(string eventId, EventRegistrationRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                throw new Exception(RegValidation.EventIdRequired);
            }

            if (string.IsNullOrWhiteSpace(request.AttendeeName))
            {
                throw new Exception(RegValidation.AttendeeNameRequired);
            }

            if (string.IsNullOrWhiteSpace(request.AttendeeEmail))
            {
                throw new Exception(RegValidation.AttendeeEmailRequired);
            }

            if (!new EmailAddressAttribute()
                    .IsValid(request.AttendeeEmail))
            {
                throw new Exception(RegValidation.InvalidEmail);
            }

            if (request.Quantity <= 0)
            {
                throw new Exception(RegValidation.InvalidQuantity);
            }

            var eventEntity = await _eventRepository.GetByIdAsync(eventId, cancellationToken);

            if (eventEntity == null)
            {
                throw new Exception(ValidationConstant.EventNotFound);
            }

            if (eventEntity.Status != EventStatus.Published.ToString())
            {
                throw new Exception(RegValidation.EventNotOpenForRegistration);
            }

            var existingRegistration =
                await _registrationRepository.GetByEmailAndEventAsync(
                    request.AttendeeEmail,
                    eventId,
                    cancellationToken);

            if (existingRegistration != null)
            {       
                throw new Exception(
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
                throw new Exception(RegValidation.RegistrationNotFound);
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
                throw new Exception(RegValidation.RegistrationNotFound);
            }

            if (registration.Status == RegistrationStatus.Cancelled.ToString())
            {
                throw new Exception(RegValidation.RegistrationAlreadyCancelled);
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

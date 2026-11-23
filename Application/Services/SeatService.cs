using Azure.Core;
using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Exceptions;
using HackDemoAPI.Application.Interfaces.IRepository;
using HackDemoAPI.Application.Interfaces.IService;
using HackDemoAPI.Application.Interfaces.Repositories;
using HackDemoAPI.Common;
using HackDemoAPI.Domain.Entities;

namespace HackDemoAPI.Application.Services
{
    public class SeatService : ISeatService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IIdempotencyRepository _idempotencyRepository;
        private readonly IWorkflowRepository _workflowRepository;

        public SeatService(
            IEventRepository eventRepository,
            IRegistrationRepository registrationRepository,
            IIdempotencyRepository idempotencyRepository, IWorkflowRepository workflowRepository)
        {
            _eventRepository = eventRepository;
            _registrationRepository = registrationRepository;
            _idempotencyRepository = idempotencyRepository;
            _workflowRepository = workflowRepository;
        }

        public async Task<SeatReservationResponse> ReserveAsync(string eventId, string idempotencyKey, SeatRequest request, CancellationToken cancellationToken)
        {
            var context = await ValidateAndGetEventAsync(eventId, idempotencyKey, ValidationConstant.RESERVE, cancellationToken);

            if (context.DuplicateResponse != null)
            {
                return context.DuplicateResponse;
            }

            var eventEntity = context.Event;
            // Check if there are enough available seats for the requested quantity
            if (eventEntity.AvailableSeats < request.Quantity)
            {  
                await UpdateRegistrationAndWorkflowAsync(eventId, idempotencyKey, request.RegistrationId, ValidationConstant.Waitlisted, cancellationToken);

                throw new ValidationException(ValidationConstant.NoSeatsAvailable);
            }                

            eventEntity.ReservedSeats += request.Quantity;
            eventEntity.AvailableSeats = eventEntity.Capacity - eventEntity.ReservedSeats;
            //update the event with new reserved seats and available seats
            await _eventRepository.UpdateAsync(eventEntity, cancellationToken);

            await UpdateRegistrationAndWorkflowAsync(eventId, idempotencyKey, request.RegistrationId, ValidationConstant.Reserved, cancellationToken);           

            return new SeatReservationResponse
            {
                EventId = eventId,
                ReservedSeats = eventEntity.ReservedSeats,
                AvailableSeats = eventEntity.Capacity - eventEntity.ReservedSeats,
                Status = ValidationConstant.Reserved
            };
        }

        private async Task UpdateRegistrationAndWorkflowAsync(string eventId, string idempotencyKey, string registrationId, string status, CancellationToken cancellationToken)
        {
            await _idempotencyRepository.UpsertAsync(new IdempotencyRecord
            {
                IdempotencyKey = idempotencyKey,
                EventId = eventId,
                Operation = status,
                CreatedOn = DateTime.UtcNow
            },
              cancellationToken);

            // If the status is "Released", we change it to "Cancelled" for the registration and workflow update
            if (status.ToLower().Equals(ValidationConstant.Released))
                status = ValidationConstant.Cancelled;

            //update the registration with reserved status
            await _registrationRepository.UpdateAsync(new Registration
            {
                RegistrationId = registrationId,
                Status = status
            },
            cancellationToken);

            await _workflowRepository.UpdateAsync(new WorkflowInstance
            {
                RegistrationId = registrationId,
                CurrentState = status,
                LastUpdatedOn = DateTime.UtcNow
            }, cancellationToken);
        }

        public async Task<SeatReservationResponse> ReleaseAsync(string eventId, string idempotencyKey, SeatRequest request,
            CancellationToken cancellationToken)
        {
            var context = await ValidateAndGetEventAsync(eventId, idempotencyKey, ValidationConstant.RELEASE, cancellationToken);

            if (context.DuplicateResponse != null)
            {
                return context.DuplicateResponse;
            }

            var eventEntity = context.Event;

            eventEntity.ReservedSeats = Math.Max(eventEntity.ReservedSeats - request.Quantity, 0);
            eventEntity.AvailableSeats = eventEntity.Capacity - eventEntity.ReservedSeats;

            await _eventRepository.UpdateAsync(eventEntity,cancellationToken);

            await UpdateRegistrationAndWorkflowAsync(eventId, idempotencyKey, request.RegistrationId, ValidationConstant.Released, cancellationToken);

            return new SeatReservationResponse
            {
                EventId = eventId,
                ReservedSeats = eventEntity.ReservedSeats,
                AvailableSeats =
                    eventEntity.Capacity -
                    eventEntity.ReservedSeats,
                Status = ValidationConstant.Released
            };
        }

        private async Task<SeatOperationContext> ValidateAndGetEventAsync(string eventId, string idempotencyKey, string operation, CancellationToken cancellationToken)
        {
            var existingRecord = await _idempotencyRepository.ExistsAsync(idempotencyKey, operation, cancellationToken);
            //Only for release this condition can be validated
            if (operation.Equals(ValidationConstant.RELEASE) && existingRecord == null)
            {
                throw new NotFoundException(
                    ValidationConstant.ReservationNotFound);
            }
            //For the same purpose of operation, validation should say already processed
            if (existingRecord != null && existingRecord.Operation == operation)
            {
                return new SeatOperationContext
                {
                    DuplicateResponse = new SeatReservationResponse
                    {
                        EventId = eventId,
                        Status = ValidationConstant.AlreadyProcessed,
                        IsDuplicateRequest = true
                    }
                };
            }

            var eventEntity = await _eventRepository.GetByIdAsync(eventId, cancellationToken);

            if (eventEntity == null)
            {
                throw new NotFoundException(ValidationConstant.EventNotFound);
            }

            return new SeatOperationContext
            {
                Event = eventEntity
            };
        }
    }
}

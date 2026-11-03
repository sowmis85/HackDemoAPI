using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Exceptions;
using HackDemoAPI.Application.Interfaces.IRepository;
using HackDemoAPI.Application.Interfaces.IService;
using HackDemoAPI.Common;

namespace HackDemoAPI.Application.Services
{
    public class SeatService : ISeatService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IIdempotencyRepository _idempotencyRepository;

        public SeatService(
            IEventRepository eventRepository,
            IIdempotencyRepository idempotencyRepository)
        {
            _eventRepository = eventRepository;
            _idempotencyRepository = idempotencyRepository;
        }

        public async Task<SeatReservationResponse> ReserveAsync(string eventId, string idempotencyKey, SeatRequest request, CancellationToken cancellationToken)
        {
            var context = await ValidateAndGetEventAsync(eventId, idempotencyKey, ValidationConstant.RESERVE, cancellationToken);

            if (context.DuplicateResponse != null)
            {
                return context.DuplicateResponse;
            }

            var eventEntity = context.Event;

            var availableSeats = eventEntity.Capacity - eventEntity.ReservedSeats;

            if (availableSeats < request.Quantity)
                throw new ValidationException(ValidationConstant.NoSeatsAvailable);

            eventEntity.ReservedSeats += request.Quantity;

            await _eventRepository.UpdateAsync(eventEntity, cancellationToken);

            await _idempotencyRepository.UpsertAsync(new IdempotencyRecord
                {
                    IdempotencyKey = idempotencyKey,
                    EventId = eventId,
                    Operation = ValidationConstant.RESERVE,
                    CreatedOn = DateTime.UtcNow
                },
                cancellationToken);

            return new SeatReservationResponse
            {
                EventId = eventId,
                ReservedSeats = eventEntity.ReservedSeats,
                AvailableSeats = eventEntity.Capacity - eventEntity.ReservedSeats,
                Status = ValidationConstant.Reserved
            };
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

            await _eventRepository.UpdateAsync(eventEntity,cancellationToken);

            await _idempotencyRepository.UpsertAsync(
                new IdempotencyRecord
                {
                    IdempotencyKey = idempotencyKey,
                    EventId = eventId,
                    Operation = ValidationConstant.RELEASE,
                    CreatedOn = DateTime.UtcNow
                },
                cancellationToken);

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

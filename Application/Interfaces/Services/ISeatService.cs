using HackDemoAPI.Application.DTO.Requests;
using HackDemoAPI.Application.DTO.Responses;

namespace HackDemoAPI.Application.Interfaces.IService
{
    public interface ISeatService
    {
        Task<SeatReservationResponse> ReserveAsync(string eventId, string idempotencyKey, SeatRequest request, CancellationToken cancellationToken);

        Task<SeatReservationResponse> ReleaseAsync(string eventId, string idempotencyKey, SeatRequest request, CancellationToken cancellationToken);
    }
}

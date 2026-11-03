using HackDemoAPI.Domain.Entities;

namespace HackDemoAPI.Application.DTO.Responses
{
    public class SeatOperationContext
    {
        public Event Event { get; set; } = default!;

        public SeatReservationResponse? DuplicateResponse { get; set; }
    }
}

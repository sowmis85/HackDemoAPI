namespace HackDemoAPI.Application.DTO.Responses
{
    public class SeatReservationResponse
    {
        public string EventId { get; set; } = string.Empty;

        public int ReservedSeats { get; set; }

        public int AvailableSeats { get; set; }

        public string Status { get; set; } = string.Empty;

        public bool IsDuplicateRequest { get; set; }
    }
}

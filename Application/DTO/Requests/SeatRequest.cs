namespace HackDemoAPI.Application.DTO.Requests
{
    public class SeatRequest
    {
        public string RegistrationId { get; set; } = string.Empty;

        public int Quantity { get; set; } = 1;
    }
}

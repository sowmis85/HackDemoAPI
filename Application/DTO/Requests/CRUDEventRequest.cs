using System.ComponentModel.DataAnnotations;

namespace HackDemoAPI.Application.DTO.Requests
{
    public class CRUDEventRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }
       
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        public int Capacity { get; set; }
    }
}

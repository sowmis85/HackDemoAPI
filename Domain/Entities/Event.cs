using HackDemoAPI.Common;
using System;

namespace HackDemoAPI.Domain.Entities
{
    public class Event
    {
        public string EventId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int ReservedSeats { get; set; }
        public int AvailableSeats { get; set; }
        public string OrganizerId { get; set; } = string.Empty;

        public DateTimeOffset CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class Venue
    {
        public string VenueId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string Mode { get; set; } = default!; // InPerson, Virtual, Hybrid
    }

    public class Speaker
    {
        public string SpeakerId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Topic { get; set; } = default!;
    }

    public class AgendaItem
    {
        public string StartTime { get; set; } = default!;
        public string EndTime { get; set; } = default!;
        public string Title { get; set; } = default!;
    }
}

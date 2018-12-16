using System;

namespace Tamagotchi.Competition.Models
{
    public class EventViewModel
    {
        public long EventId { get; set; }
        public string ActionCode { get; set; }
        public string DeseaseCode { get; set; }
        public TimeSpan StartDate { get; set; }
        public TimeSpan EndDate { get; set; }
        public int Value { get; set; }
    }
}

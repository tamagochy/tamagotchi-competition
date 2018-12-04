using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tamagotchi.Competition.Models
{
    public class EventViewModel
    {
        public long EventId { get; set; }
        public string ActionCode { get; set; }
        public string DeseaseCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Value { get; set; }
    }
}

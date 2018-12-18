using System;
using System.ComponentModel.DataAnnotations;

namespace Tamagotchi.Competition.Context
{
    public partial class Events
    {
        [Key]
        public long Id { get; set; }
        public string ActionCode { get; set; }
        public string RoomCode { get; set; }
        public string DeseaseCode { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan Finish { get; set; }
        public int Value { get; set; }
    }
}

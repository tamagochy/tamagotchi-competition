
using Newtonsoft.Json;
using System;

namespace Tamagotchi.Competition.Models
{
    public class ScoreViewModel
    {
        public long ScoreId { get; set; }
        public long UserId { get; set; }        
        public string Login { get; set; }
        public int Value { get; set; }
    }

    public class ScoreParam : ScoreViewModel
    {
        public string ActionCode { get; set; }
        public string RoomCode { get; set; }
        public string DeseaseCode { get; set; }
        public DateTime Time { get; set; }
    }

}

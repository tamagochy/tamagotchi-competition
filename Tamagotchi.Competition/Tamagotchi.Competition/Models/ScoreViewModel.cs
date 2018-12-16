using Newtonsoft.Json;
using System;

namespace Tamagotchi.Competition.Models
{
    public class ScoreViewModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? ScoreId { get; set; }
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? UserId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Login { get; set; }
        public int Value { get; set; }
    }

    public class ScoreParam : ScoreViewModel
    {       
        public string ActionCode { get; set; }
        public string RoomCode { get; set; }
        public string DeseaseCode { get; set; }
        public string Time { get; set; }
        public TimeSpan EventDate { get; set; }
    }

}

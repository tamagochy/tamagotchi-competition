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

    public class ScoreParam
    {
        [JsonProperty("actionCode")]
        public string ActionCode { get; set; }
        [JsonProperty("roomCode")]
        public string RoomCode { get; set; }
        [JsonProperty("deseaseCode")]
        public string DeseaseCode { get; set; }
        [JsonProperty("time")]
        public string Time { get; set; }
        [JsonIgnore]
        public TimeSpan EventTime { get; set; }
        [JsonIgnore]
        public long UserId { get; set; }
    }

}

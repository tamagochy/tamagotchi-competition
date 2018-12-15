using Newtonsoft.Json;

namespace Tamagotchi.Competition.Models
{
    public class SuccessResult
    {
        [JsonProperty("succeed")]
        public bool Succeed { get; set; }
    }
}

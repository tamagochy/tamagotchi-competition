using Newtonsoft.Json;

namespace Tamagotchi.Competition.Helpers.API
{
    public class SuccessResult 
    {
        [JsonProperty("succeed")]
        public bool Succeed { get; set; }
    }
}

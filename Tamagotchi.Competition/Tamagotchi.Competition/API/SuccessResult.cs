using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tamagotchi.Competition.API
{
    public class SuccessResult
    {
        [JsonProperty("succeed")]
        public bool Succeed { get; set; }
    }
}

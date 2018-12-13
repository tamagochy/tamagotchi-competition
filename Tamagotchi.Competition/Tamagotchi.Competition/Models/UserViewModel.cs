
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tamagotchi.Competition.Models
{
    public class UserViewModel
    {
        public long UserId { get; set; }
        public string UerName { get; set; }
    }

    public class UserParamModel
    {
        public string UserId { get; set; }
    }

    public class BaseAuthEntity
    {
        [JsonProperty("data")]
        public List<AuthUserEntity> Users { get; set; }
    }

    public class AuthUserEntity
    {
        [JsonProperty("user_id")]
        public long UserId { get; set; }
        [JsonProperty("user_login")]
        public string Login { get; set; }
    }

}

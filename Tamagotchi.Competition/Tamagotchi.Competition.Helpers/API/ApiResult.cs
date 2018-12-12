using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tamagotchi.Competition.Helpers.API
{
    public class ApiResult<T> where T : class
    {
        public ApiResult()
        { }

        public ApiResult(T data)
        {
            Data = data;
        }

        public ApiResult(List<Error> errors)
        {
            Errors = errors;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public string Message { get; set; }
    }

    public class Error<T> : Error where T : class
    {
        public T AdditionalData { get; set; }
    }
}

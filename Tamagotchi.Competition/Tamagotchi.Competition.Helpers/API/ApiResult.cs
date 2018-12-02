﻿
namespace Tamagotchi.Competition.Helpers.API
{
    public class ApiResult<T> where T : class
    {
        public ApiResult()
        { }

        public ApiResult(T result)
        {
            Result = result;
        }

        public ApiResult(Error error)
        {
            Error = error;
        }

        public T Result { get; set; }
        public Error Error { get; set; }
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
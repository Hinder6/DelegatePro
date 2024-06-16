using System;
using Newtonsoft.Json;

namespace DelegatePro.PCL
{
    [JsonObject]
    public class APIResponse : IDisposable
    {
        public string Message { get; set; }
        public bool Result { get; set; }

        public static APIResponse CreateAsFailure(string message)
        {
            return new APIResponse
            {
                Message = message,
                Result = false
            };
        }

        public static APIResponse CreateAsSuccess()
        {
            return new APIResponse
            {
                Result = true
            };
        }

        public static APIResponse<T> CreateAsFailure<T>(string message)
        {
            return new APIResponse<T>
            {
                Message = message,
                Result = false
            };
        }

        public static APIResponse<T> CreateWithData<T>(T data)
        {
            return new APIResponse<T>
            {
                Result = true,
                Data = data
            };
        }

        public virtual void Dispose() { }
    }

    [JsonObject]
    public class APIResponse<T> : APIResponse
    {
        public T Data { get; set; }
    }
}


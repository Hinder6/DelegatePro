using System;
namespace DelegatePro.PCL
{
    public class Result
    {
        public string Message { get; set; }
        public bool Success { get; set; }

        public static Result CreateAsFailure(string message)
        {
            return new Result { Success = false, Message = message };
        }

        public static Result CreateAsSuccess()
        {
            return new Result { Success = true };
        }
    }
}


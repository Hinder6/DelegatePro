﻿using System;
namespace DelegatePro.PCL
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }

        public static ValidationResult AsValid()
        {
            return new ValidationResult { IsValid = true };
        }

        public static ValidationResult AsFailed(string message)
        {
            return new ValidationResult { IsValid = false, Message = message };
        }
    }
}


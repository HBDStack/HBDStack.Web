using System;

namespace HBD.AspNetCore.Tests.Exceptions;

public class InvalidArgumentException : ArgumentException
{
    public InvalidArgumentException(string errorMessage) : base(errorMessage)
    {
    }
}
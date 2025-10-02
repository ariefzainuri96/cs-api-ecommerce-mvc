using System;

namespace Ecommerce.GlobalException;

/// <summary>
/// Custom exception for when a client specifies an invalid property name for filtering or ordering.
/// </summary>
public class InvalidPropertyException : ArgumentException
{
    public InvalidPropertyException(string message) : base(message) { }

    // Allows chaining the inner exception if one occurred during reflection
    public InvalidPropertyException(string message, Exception innerException) : base(message, innerException) { }
}

using System;

public class KeyExistsException : Exception
{
    public KeyExistsException()
    {
    }

    public KeyExistsException(string message)
        : base(message)
    {
    }

    public KeyExistsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
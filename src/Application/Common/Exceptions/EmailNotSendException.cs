namespace Application.Common.Exceptions;

public class EmailNotSendException : Exception
{
    public EmailNotSendException(string message)
        : base(message)
    {
    }

    public EmailNotSendException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
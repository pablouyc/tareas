namespace Tareas.Api.Application.Exceptions;

public class DomainValidationException : Exception
{
    public DomainValidationException(string message, object? details = null) : base(message)
    {
        Details = details;
    }

    public object? Details { get; }
}

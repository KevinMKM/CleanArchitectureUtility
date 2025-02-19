namespace CleanArchitectureUtility.Core.Domain.Exceptions;

public class InvalidValueObjectStateException : DomainStateException
{
    public InvalidValueObjectStateException(string message) : base(message)
    {
    }
}
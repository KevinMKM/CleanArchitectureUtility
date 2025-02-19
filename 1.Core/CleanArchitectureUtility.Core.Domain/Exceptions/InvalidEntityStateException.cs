namespace CleanArchitectureUtility.Core.Domain.Exceptions;

public class InvalidEntityStateException : DomainStateException
{
    public InvalidEntityStateException(string message) : base(message)
    {
    }
}
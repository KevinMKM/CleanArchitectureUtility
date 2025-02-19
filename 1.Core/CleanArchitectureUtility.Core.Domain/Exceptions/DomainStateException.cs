namespace CleanArchitectureUtility.Core.Domain.Exceptions;

public abstract class DomainStateException : Exception
{
    protected DomainStateException(string message) : base(message)
    {
    }
}
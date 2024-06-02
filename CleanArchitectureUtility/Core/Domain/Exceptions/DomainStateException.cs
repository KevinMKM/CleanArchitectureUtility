namespace CleanArchitectureUtility.Core.Domain.Exceptions;

public abstract class DomainStateException : Exception
{
    public string[] Parameters { get; set; }

    protected DomainStateException(string message, params string[] parameters) : base(message)
    {
        Parameters = parameters;
    }

    public override string ToString()
    {
        if (Parameters?.Length < 1)
            return Message;

        var result = Message;

        for (var i = 0; i < Parameters?.Length; i++)
        {
            var placeHolder = $"{{{i}}}";
            result = result.Replace(placeHolder, Parameters[i]);
        }

        return result;
    }
}
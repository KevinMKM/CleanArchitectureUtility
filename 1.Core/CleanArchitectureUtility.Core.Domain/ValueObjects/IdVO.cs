using CleanArchitectureUtility.Core.Domain.Exceptions;

namespace CleanArchitectureUtility.Core.Domain.ValueObjects;

public class IdVO : BaseValueObject<IdVO>
{
    public Guid Value { get; }

    private IdVO()
    {
    }

    public IdVO(Guid value)
    {
        Validate(value);
        Value = value;
    }

    public IdVO(string value)
    {
        Validate(value);
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidValueObjectStateException($"{nameof(IdVO)} can't be null");

        if (Guid.TryParse(value, out var parsedValue))
            Value = parsedValue;
        else
            throw new InvalidValueObjectStateException($"Value of {nameof(IdVO)} is invalid");
    }

    protected virtual void Validate(Guid value)
    {
    }

    protected virtual void Validate(string value)
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static IdVO FromString(string value) => new(value);
    public static IdVO FromGuid(Guid value) => new(value);
    public static IdVO NewId() => new(Guid.NewGuid());
    public static IdVO Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
    public static explicit operator string(IdVO vo) => vo.Value.ToString();
    public static implicit operator IdVO(string value) => new(value);
    public static explicit operator Guid(IdVO vo) => vo.Value;
    public static implicit operator IdVO(Guid value) => new(value);
}
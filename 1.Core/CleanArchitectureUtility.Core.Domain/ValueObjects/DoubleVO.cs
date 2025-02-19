namespace CleanArchitectureUtility.Core.Domain.ValueObjects;

public class DoubleVO : BaseValueObject<DoubleVO>
{
    public double Value { get; }

    public DoubleVO(double value)
    {
        Validate(value);
        Value = value;
    }

    protected virtual void Validate(double value)
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static explicit operator double(DoubleVO vo) => vo.Value;
    public static implicit operator DoubleVO(double value) => new DoubleVO(value);
}
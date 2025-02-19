namespace CleanArchitectureUtility.Core.Domain.ValueObjects;

public class IntegerVO : BaseValueObject<IntegerVO>
{
    public int Value { get; }

    public IntegerVO(int value)
    {
        Validate(value);
        Value = value;
    }

    protected virtual void Validate(int value)
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static explicit operator int(IntegerVO vo) => vo.Value;
    public static implicit operator IntegerVO(int value) => new IntegerVO(value);
}
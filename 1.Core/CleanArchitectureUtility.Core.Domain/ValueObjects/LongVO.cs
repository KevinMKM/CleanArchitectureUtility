namespace CleanArchitectureUtility.Core.Domain.ValueObjects;

public class LongVO : BaseValueObject<LongVO>
{
    public long Value { get; }

    public LongVO(long value)
    {
        Validate(value);
        Value = value;
    }

    protected virtual void Validate(long value)
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static explicit operator long(LongVO vo) => vo.Value;
    public static implicit operator LongVO(long value) => new LongVO(value);
}
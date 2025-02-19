namespace CleanArchitectureUtility.Core.Domain.ValueObjects;

public class ShortVO : BaseValueObject<ShortVO>
{
    public short Value { get; }

    public ShortVO(short value)
    {
        Validate(value);
        Value = value;
    }

    protected virtual void Validate(short value)
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static explicit operator short(ShortVO vo) => vo.Value;
    public static implicit operator ShortVO(short value) => new ShortVO(value);
}
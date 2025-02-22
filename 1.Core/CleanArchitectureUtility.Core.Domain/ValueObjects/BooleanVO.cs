namespace CleanArchitectureUtility.Core.Domain.ValueObjects;

public class BooleanVO : BaseValueObject<BooleanVO>
{
    public bool Value { get; }

    private BooleanVO()
    {
    }

    public BooleanVO(bool value)
    {
        Validate(value);
        Value = value;
    }

    protected virtual void Validate(bool value)
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static BooleanVO True() => new BooleanVO(true);
    public static BooleanVO False() => new BooleanVO(false);
    public static explicit operator bool(BooleanVO vo) => vo.Value;
    public static implicit operator BooleanVO(bool value) => new BooleanVO(value);
}
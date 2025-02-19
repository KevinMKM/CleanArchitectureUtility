namespace CleanArchitectureUtility.Core.Domain.ValueObjects;

public class DateTimeVO : BaseValueObject<DateTimeVO>
{
    public DateTime Value { get; }

    public DateTimeVO(DateTime value)
    {
        Validate(value);
        Value = value;
    }

    protected virtual void Validate(DateTime value)
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static DateTimeVO Now() => new DateTimeVO(DateTime.UtcNow);
    public static DateTimeVO FromDateTime(DateTime value) => new DateTimeVO(value);
    public static explicit operator DateTime(DateTimeVO vo) => vo.Value;
    public static implicit operator DateTimeVO(DateTime value) => new DateTimeVO(value);
}
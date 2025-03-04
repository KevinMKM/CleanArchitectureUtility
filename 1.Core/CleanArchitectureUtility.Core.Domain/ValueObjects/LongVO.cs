﻿namespace CleanArchitectureUtility.Core.Domain.ValueObjects;

public class LongVO : BaseValueObject<LongVO>
{
    public long Value { get; }

    private LongVO()
    {
    }

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

    public static LongVO FromLong(long value) => new LongVO(value);
    public static explicit operator long(LongVO vo) => vo.Value;
    public static implicit operator LongVO(long value) => new LongVO(value);
}
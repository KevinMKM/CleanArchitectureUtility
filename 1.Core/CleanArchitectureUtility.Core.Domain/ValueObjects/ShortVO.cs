﻿namespace CleanArchitectureUtility.Core.Domain.ValueObjects;

public class ShortVO : BaseValueObject<ShortVO>
{
    public short Value { get; }

    private ShortVO()
    {
    }

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

    public static ShortVO FromShort(short value) => new ShortVO(value);
    public static explicit operator short(ShortVO vo) => vo.Value;
    public static implicit operator ShortVO(short value) => new ShortVO(value);
}
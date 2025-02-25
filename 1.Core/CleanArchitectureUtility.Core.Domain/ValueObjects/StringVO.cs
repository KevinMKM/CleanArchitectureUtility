﻿namespace CleanArchitectureUtility.Core.Domain.ValueObjects;

public class StringVO : BaseValueObject<StringVO>
{
    public string Value { get; }

    private StringVO()
    {
    }

    public StringVO(string value)
    {
        Validate(value);
        Value = value;
    }

    protected virtual void Validate(string value)
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static StringVO FromString(string value) => new StringVO(value);
    public static explicit operator string(StringVO vo) => vo.Value;
    public static implicit operator StringVO(string value) => new StringVO(value);
}
﻿namespace CleanArchitectureUtility.Core.Domain.ValueObjects;

public class FloatVO : BaseValueObject<FloatVO>
{
    public float Value { get; }

    private FloatVO()
    {
    }

    public FloatVO(float value)
    {
        Validate(value);
        Value = value;
    }

    protected virtual void Validate(float value)
    {
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static FloatVO FromFloat(float value) => new FloatVO(value);
    public static explicit operator float(FloatVO vo) => vo.Value;
    public static implicit operator FloatVO(float value) => new FloatVO(value);
}
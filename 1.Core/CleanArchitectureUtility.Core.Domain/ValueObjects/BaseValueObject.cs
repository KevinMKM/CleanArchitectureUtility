namespace CleanArchitectureUtility.Core.Domain.ValueObjects;

public abstract class BaseValueObject<TValueObject> : IEquatable<TValueObject> where TValueObject : BaseValueObject<TValueObject>
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public bool Equals(TValueObject? other) => other != null && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    
    public override bool Equals(object? obj) => obj is TValueObject otherObject && Equals(otherObject);
    
    public override int GetHashCode() => HashCode.Combine(GetEqualityComponents().Select(x => x?.GetHashCode() ?? 0).ToArray());
    
    public static bool operator ==(BaseValueObject<TValueObject>? left, BaseValueObject<TValueObject>? right) => left?.Equals(right) ?? right is null;

    public static bool operator !=(BaseValueObject<TValueObject>? left, BaseValueObject<TValueObject>? right) => !(left == right);
}
using CleanArchitectureUtility.Core.Domain.ValueObjects;

namespace CleanArchitectureUtility.Core.Domain.Entities;

public abstract class BaseEntity
{
    public IdVO Id { get; }

    public DateTimeVO? RemoveDateTime { get; private set; }
    public IdVO? RemoverUserId { get; private set; }

    public BooleanVO IsDeleted { get; private set; }
    public BooleanVO IsActive { get; private set; }

    #region Constructors

    protected BaseEntity()
    {
        Id = IdVO.NewId();
        IsDeleted = BooleanVO.False();
        IsActive = BooleanVO.True();
    }

    #endregion

    #region Update

    public void Activate(Guid updaterUserId, DateTime updaterDateTime)
    {
        IsActive = BooleanVO.True();
    }

    public void Deactivate(Guid updaterUserId, DateTime updaterDateTime)
    {
        IsActive = BooleanVO.False();
    }

    #endregion

    #region Delete

    private void SetRemoverDetails(Guid removerUserId, DateTime removeDateTime)
    {
        RemoverUserId = IdVO.FromGuid(removerUserId);
        RemoveDateTime = DateTimeVO.FromDateTime(removeDateTime);
    }

    public void Remove(Guid removerUserId, DateTime removeDateTime)
    {
        IsDeleted = BooleanVO.True();
        SetRemoverDetails(removerUserId, removeDateTime);
    }

    public void Recover(Guid recoverUserId, DateTime recoveryDateTime)
    {
        IsDeleted = BooleanVO.False();
        RemoveDateTime = null;
        RemoverUserId = null;
    }

    #endregion

    #region Equality Check

    public bool Equals(BaseEntity? other) => this == other;

    public override bool Equals(object? obj) => obj is BaseEntity otherObject && Id.Equals(otherObject.Id);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(BaseEntity? left, BaseEntity? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(BaseEntity left, BaseEntity right) => !(right == left);

    #endregion
}
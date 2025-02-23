using CleanArchitectureUtility.Core.Domain.ValueObjects;

namespace CleanArchitectureUtility.Core.Domain.Entities;

public abstract class BaseEntity
{
    public IdVO Id { get; }

    public DateTimeVO? CreateDateTime { get; private set; }
    public IdVO? CreatorUserId { get; private set; }

    public DateTimeVO? UpdateDateTime { get; private set; }
    public IdVO? UpdaterUserId { get; private set; }

    public DateTimeVO? RemoveDateTime { get; private set; }
    public IdVO? RemoverUserId { get; private set; }

    public BooleanVO IsDeleted { get; private set; }
    public BooleanVO IsActive { get; private set; }
    public byte[] RowVersion { get; protected set; } = Array.Empty<byte>();

    #region Constructors

    protected BaseEntity() : this(Guid.Empty)
    {
    }

    protected BaseEntity(Guid creatorUserId)
    {
        Id = IdVO.NewId();
        IsDeleted = BooleanVO.False();
        IsActive = BooleanVO.True();

        if (creatorUserId == Guid.Empty)
            return;
        var now = DateTime.UtcNow;
        SetCreatorDetails(creatorUserId, now);
        SetUpdaterDetails(creatorUserId, now);
    }

    #endregion

    #region Create

    public void SetCreatorDetails(Guid creatorUserId, DateTime createDateTime)
    {
        if (CreatorUserId is not null)
            return;
        CreatorUserId = IdVO.FromGuid(creatorUserId);
        CreateDateTime = DateTimeVO.FromDateTime(createDateTime);
    }

    #endregion

    #region Update

    public void SetUpdaterDetails(Guid updaterUserId, DateTime updateDateTime)
    {
        UpdaterUserId = IdVO.FromGuid(updaterUserId);
        UpdateDateTime = DateTimeVO.FromDateTime(updateDateTime);
    }

    public void SetActiveState(bool isActive, Guid updaterUserId, DateTime updaterDateTime)
    {
        if (IsActive == BooleanVO.FromBoolean(isActive))
            return;
        IsActive = BooleanVO.FromBoolean(isActive);
        SetUpdaterDetails(updaterUserId, updaterDateTime);
    }

    #endregion

    #region Delete / Recovery

    public void SetRemoverDetails(Guid removerUserId, DateTime removeDateTime)
    {
        RemoverUserId = IdVO.FromGuid(removerUserId);
        RemoveDateTime = DateTimeVO.FromDateTime(removeDateTime);
    }

    public void SetDeletionState(bool isDeleted, Guid userId, DateTime actionDateTime)
    {
        if (IsDeleted == BooleanVO.FromBoolean(isDeleted))
            return;
        IsDeleted = BooleanVO.FromBoolean(isDeleted);

        if (isDeleted)
            SetRemoverDetails(userId, actionDateTime);
        else
        {
            RemoveDateTime = null;
            RemoverUserId = null;
        }
    }

    #endregion

    #region Equality Check

    public override bool Equals(object? obj) => obj is BaseEntity other && Id.Equals(other.Id);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(BaseEntity? left, BaseEntity? right)
        => ReferenceEquals(left, right) || (left is not null && right is not null && left.Id == right.Id);

    public static bool operator !=(BaseEntity? left, BaseEntity? right) => !(left == right);

    #endregion
}
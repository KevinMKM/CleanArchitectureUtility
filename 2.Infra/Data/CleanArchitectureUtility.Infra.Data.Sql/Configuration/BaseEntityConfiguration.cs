using CleanArchitectureUtility.Core.Domain.Entities;
using CleanArchitectureUtility.Core.Domain.ValueObjects;

namespace CleanArchitectureUtility.Infra.Data.Sql.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasConversion(id => id.Value, value => IdVO.FromGuid(value))
            .ValueGeneratedNever();
        builder.Property(e => e.IsDeleted)
            .HasConversion(isDeleted => isDeleted.Value, value => BooleanVO.FromBoolean(value));
        builder.Property(e => e.IsActive)
            .HasConversion(isActive => isActive.Value, value => BooleanVO.FromBoolean(value));
        builder.Property(e => e.CreateDateTime)
            .HasConversion(createDate => createDate.Value, value => DateTimeVO.FromDateTime(value));
        builder.Property(e => e.UpdateDateTime)
            .HasConversion(updateDate => updateDate.Value, value => DateTimeVO.FromDateTime(value));
        builder.Property(e => e.RemoveDateTime)
            .HasConversion(removeDate => removeDate.Value, value => DateTimeVO.FromDateTime(value));
        builder.Property(e => e.CreatorUserId)
            .HasConversion(creatorId => creatorId.Value, value => IdVO.FromGuid(value));
        builder.Property(e => e.UpdaterUserId)
            .HasConversion(updaterId => updaterId.Value, value => IdVO.FromGuid(value));
        builder.Property(e => e.RemoverUserId)
            .HasConversion(removerId => removerId.Value, value => IdVO.FromGuid(value));
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}
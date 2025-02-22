using CleanArchitectureUtility.Core.Domain.Entities;
using CleanArchitectureUtility.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitectureUtility.Infra.Data.SqlCommands.Extensions;

public static class ModelBuilderExtensions
{
    public static void AddBusinessId(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(e => typeof(AggregateRoot).IsAssignableFrom(e.ClrType) || typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .Property<IdVO>("Id").HasConversion(c => c.Value, d => IdVO.FromGuid(d))
                .IsUnicode()
                .IsRequired();
            modelBuilder.Entity(entityType.ClrType).HasAlternateKey("Id");
        }
    }
}
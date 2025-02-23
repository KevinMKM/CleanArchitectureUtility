using CleanArchitectureUtility.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitectureUtility.Infra.Data.Sql.ValueConversions;

public class LongConversion : ValueConverter<LongVO, long>
{
    public LongConversion() : base(c => c.Value, c => LongVO.FromLong(c))
    {
    }
}
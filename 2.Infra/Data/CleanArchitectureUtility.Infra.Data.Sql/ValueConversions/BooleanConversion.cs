using CleanArchitectureUtility.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitectureUtility.Infra.Data.Sql.ValueConversions;

public class BooleanConversion : ValueConverter<BooleanVO, bool>
{
    public BooleanConversion() : base(c => c.Value, c => BooleanVO.FromBoolean(c))
    {
    }
}
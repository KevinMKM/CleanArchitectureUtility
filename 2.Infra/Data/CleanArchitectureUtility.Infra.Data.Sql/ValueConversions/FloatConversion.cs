using CleanArchitectureUtility.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitectureUtility.Infra.Data.Sql.ValueConversions;

public class FloatConversion : ValueConverter<FloatVO, float>
{
    public FloatConversion() : base(c => c.Value, c => FloatVO.FromFloat(c))
    {
    }
}
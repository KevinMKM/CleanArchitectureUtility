using CleanArchitectureUtility.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitectureUtility.Infra.Data.Sql.ValueConversions;

public class DoubleConversion : ValueConverter<DoubleVO, double>
{
    public DoubleConversion() : base(c => c.Value, c => DoubleVO.FromDouble(c))
    {
    }
}
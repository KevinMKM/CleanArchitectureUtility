using CleanArchitectureUtility.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitectureUtility.Infra.Data.Sql.ValueConversions;

public class IntegerConversion : ValueConverter<IntegerVO, int>
{
    public IntegerConversion() : base(c => c.Value, c => IntegerVO.FromInteger(c))
    {
    }
}
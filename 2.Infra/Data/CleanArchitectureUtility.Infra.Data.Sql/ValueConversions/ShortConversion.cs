using CleanArchitectureUtility.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitectureUtility.Infra.Data.Sql.ValueConversions;

public class ShortConversion : ValueConverter<ShortVO, short>
{
    public ShortConversion() : base(c => c.Value, c => ShortVO.FromShort(c))
    {
    }
}
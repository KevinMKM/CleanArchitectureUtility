using CleanArchitectureUtility.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitectureUtility.Infra.Data.Sql.ValueConversions;

public class DateTimeConversion : ValueConverter<DateTimeVO, DateTime>
{
    public DateTimeConversion() : base(c => c.Value, c => DateTimeVO.FromDateTime(c))
    {
    }
}
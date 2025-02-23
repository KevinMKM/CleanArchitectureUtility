using CleanArchitectureUtility.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitectureUtility.Infra.Data.Sql.ValueConversions;

public class StringConversion : ValueConverter<StringVO, string>
{
    public StringConversion() : base(c => c.Value, c => StringVO.FromString(c))
    {
    }
}
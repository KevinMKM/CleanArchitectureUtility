using CleanArchitectureUtility.Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleanArchitectureUtility.Infra.Data.SqlCommands.ValueConversions;

public class IdConversion : ValueConverter<IdVO, Guid>
{
    public IdConversion() : base(c => c.Value, c => IdVO.FromGuid(c))
    {
    }
}
public class BooleanConversion : ValueConverter<BooleanVO, bool>
{
    public BooleanConversion() : base(c => c.Value, c => BooleanVO.FromBool(c))
    {
    }
}
public class IdConversion : ValueConverter<IdVO, Guid>
{
    public IdConversion() : base(c => c.Value, c => IdVO.FromGuid(c))
    {
    }
}
public class IdConversion : ValueConverter<IdVO, Guid>
{
    public IdConversion() : base(c => c.Value, c => IdVO.FromGuid(c))
    {
    }
}
public class IdConversion : ValueConverter<IdVO, Guid>
{
    public IdConversion() : base(c => c.Value, c => IdVO.FromGuid(c))
    {
    }
}
public class IdConversion : ValueConverter<IdVO, Guid>
{
    public IdConversion() : base(c => c.Value, c => IdVO.FromGuid(c))
    {
    }
}
public class IdConversion : ValueConverter<IdVO, Guid>
{
    public IdConversion() : base(c => c.Value, c => IdVO.FromGuid(c))
    {
    }
}
public class IdConversion : ValueConverter<IdVO, Guid>
{
    public IdConversion() : base(c => c.Value, c => IdVO.FromGuid(c))
    {
    }
}
public class IdConversion : ValueConverter<IdVO, Guid>
{
    public IdConversion() : base(c => c.Value, c => IdVO.FromGuid(c))
    {
    }
}
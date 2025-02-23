using CleanArchitectureUtility.Extensions.Abstractions.UsersManagements;
using CleanArchitectureUtility.Infra.Data.Sql;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureUtility.Infra.Data.SqlCommands;

public abstract class BaseCommandDbContext : BaseDbContext
{
    protected BaseCommandDbContext(DbContextOptions options, IUserInfoService userInfoService) : base(options, userInfoService)
    {
    }

    protected BaseCommandDbContext()
    {
    }
}
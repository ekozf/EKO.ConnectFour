using EKO.ConnectFour.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EKO.ConnectFour.Infrastructure.Storage;

//DO NOT TOUCH THIS FILE!!
public class ConnectFourDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    private readonly IConfiguration _configuration;

    public ConnectFourDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sqlite database
#if DEBUG
        options.UseSqlite(_configuration.GetConnectionString("ConnectFourDbConnectionDev"));
#else
        options.UseSqlite(_configuration.GetConnectionString("ConnectFourDbConnectionProd"));
#endif
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("Users");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("ExternalLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}
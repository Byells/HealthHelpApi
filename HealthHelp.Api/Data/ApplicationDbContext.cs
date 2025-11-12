using HealthHelp.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthHelp.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<RoutineEntry> RoutineEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            const string prefix = "HealthHelp_";

            builder.Entity<ApplicationUser>(b =>
            {
                b.ToTable(prefix + "Users");
                
                b.Property(u => u.EmailConfirmed).HasColumnType("NUMBER(1)");
                b.Property(u => u.PhoneNumberConfirmed).HasColumnType("NUMBER(1)");
                b.Property(u => u.TwoFactorEnabled).HasColumnType("NUMBER(1)");
                b.Property(u => u.LockoutEnabled).HasColumnType("NUMBER(1)");
                
                b.HasIndex(u => u.NormalizedUserName).HasDatabaseName(prefix + "UserNameIndex");
                b.HasIndex(u => u.NormalizedEmail).HasDatabaseName(prefix + "EmailIndex");
            });

            builder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable(prefix + "UserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.ToTable(prefix + "UserLogins");
            });

            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable(prefix + "UserTokens");
            });

            builder.Entity<IdentityRole>(b =>
            {
                b.ToTable(prefix + "Roles");
                
                b.HasIndex(r => r.NormalizedName).HasDatabaseName(prefix + "RoleNameIndex");
            });

            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable(prefix + "RoleClaims");
            });

            builder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable(prefix + "UserRoles");
            });

            builder.Entity<RoutineEntry>().Property(p => p.Hours).HasPrecision(5, 2);
        }
    }
}
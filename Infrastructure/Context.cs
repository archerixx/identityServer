using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Domain.Models.Identity;

namespace Project.Infrastructure
{
    public class Context : IdentityDbContext<Users, Roles, Guid, UserClaims, UserRoles, UserLogins, RoleClaims, UserTokens>
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<IdentityServer4.EntityFramework.Entities.PersistedGrant> PersistedGrants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            #region // Identity Server tables
            // Map entities to their new tables names
            builder.Entity<Users>(table =>
            {
                table.ToTable("Users");
                // Each User can have many UserRoles
                table.HasMany(e => e.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                // Each User can have many UserClaims
                table.HasMany(e => e.UserClaims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();
                // Each User can have many UserLogins
                table.HasMany(e => e.UserLogins)
                    .WithOne()
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                table.HasMany(e => e.UserTokens)
                    .WithOne()
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();
            });
            builder.Entity<UserClaims>(table =>
            {
                table.ToTable("UserClaims");
            });
            builder.Entity<UserLogins>(table =>
            {
                table.ToTable("UserLogins");
            });
            builder.Entity<UserTokens>(table =>
            {
                table.ToTable("UserTokens");
            });
            builder.Entity<Roles>(table =>
            {
                table.ToTable("Roles");
                // Each Role can have many UserRoles
                table.HasMany(e => e.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });
            builder.Entity<RoleClaims>(table =>
            {
                table.ToTable("RoleClaims");
            });
            builder.Entity<UserRoles>(table =>
            {
                table.ToTable("UserRoles");
            });

            builder.Entity<IdentityServer4.EntityFramework.Entities.PersistedGrant>(table =>
            {
                table.HasKey(t => t.Key);
            });
            #endregion
        }
    }
}

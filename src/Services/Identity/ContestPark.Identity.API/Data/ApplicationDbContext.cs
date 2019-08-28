using ContestPark.Identity.API.Data.Tables;
using ContestPark.Identity.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContestPark.Identity.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ReferenceCode> ReferenceCodes { get; set; }
        public DbSet<Reference> References { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Reference>().Property(x => x.Amount).HasColumnType("decimal(13,2)");

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}

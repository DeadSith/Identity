using Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<GitRepo> Repos { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            /*builder.Entity<GitRepo>()
                .HasOne(p => p.Author)
                .WithMany(b => b.Repos);*/
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Repos)
                .WithOne(r => r.Author)
                .IsRequired();
        }
    }
}
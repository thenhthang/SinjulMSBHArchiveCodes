
using EFCore5Concurrency.SinjulMSBH;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EFCore5Concurrency.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Blog> Blogs { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Actor>()
                .Property(p => p.FirstName)
                .IsConcurrencyToken()
            //.ValueGeneratedOnAddOrUpdate()
            ;

            builder.Entity<Movie>()
                .Property(p => p.Timestamp)
                .IsRowVersion()
            ;
        }
    }
}

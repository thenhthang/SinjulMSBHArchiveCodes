
using Microsoft.EntityFrameworkCore;

namespace WorkerService
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Person>()
            //    //.ToTable("People" , _ => _.ExcludeFromMigrations())
            //    .HasIndex(_ => _.Email)
            //    .IsUnique()
            //;

            modelBuilder.Entity<MediaPost>();
        }
    }
}

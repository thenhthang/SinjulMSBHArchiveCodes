using System.Collections.Generic;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using NET5RC1.SinjulMSBH;

namespace NET5RC1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //Property bags
        public DbSet<Dictionary<string, object>> Products =>
            Set<Dictionary<string, object>>("Product");
        public DbSet<Dictionary<string, object>> Categories =>
            Set<Dictionary<string, object>>("Category");

        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
          => optionsBuilder
              .AddInterceptors(new MySaveChangesInterceptor())
              .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=NET5RC1DB;Trusted_Connection=True;MultipleActiveResultSets=true")
        ;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unlike EF6, EF Core allows full customization of the join table. For example, the code below configures a many-to-many relationship that also has navigations to the join entity, and in which the join entity contains a payload property:
            //modelBuilder
            //    .Entity<Post>()
            //    .HasMany(e => e.Members)
            //    .WithMany(e => e.Memberships)
            //    .UsingEntity<PersonCommunity>(
            //        b => b.HasOne(e => e.Member).WithMany().HasForeignKey(e => e.MembersId),
            //        b => b.HasOne(e => e.Membership).WithMany().HasForeignKey(e => e.MembershipsId))
            //    .Property(e => e.MemberSince).HasDefaultValueSql("CURRENT_TIMESTAMP");


            // Map entity types to queries
            modelBuilder.Entity<Post>().ToSqlQuery(
            @"SELECT Id, Name, Category, BlogId FROM posts
                  UNION ALL
                  SELECT Id, Name, ""Legacy"", BlogId from legacy_posts")
            ;


            // Property bags
            modelBuilder.SharedTypeEntity<Dictionary<string, object>>("Category", b =>
            {
                b.IndexerProperty<string>("Description");
                b.IndexerProperty<int>("Id");
                b.IndexerProperty<string>("Name").IsRequired();
            });
            modelBuilder.SharedTypeEntity<Dictionary<string, object>>("Product", b =>
            {
                b.IndexerProperty<int>("Id");
                b.IndexerProperty<string>("Name").IsRequired();
                b.IndexerProperty<string>("Description");
                b.IndexerProperty<decimal>("Price");
                b.IndexerProperty<int?>("CategoryId");

                b.HasOne("Category", null).WithMany();
            });


            // Exclude tables from migrations
            //modelBuilder.Entity<User>().ToTable("Users", t => t.ExcludeFromMigrations());

            // Required 1:1 dependents
            modelBuilder.Entity<Person>(b =>
            {
                b.OwnsOne(e => e.HomeAddress,
                    b =>
                    {
                        b.Property(e => e.Line1).IsRequired();
                        b.Property(e => e.City).IsRequired();
                        b.Property(e => e.Region).IsRequired();
                        b.Property(e => e.Postcode).IsRequired();
                    })
                ;
                b.Navigation(e => e.HomeAddress).IsRequired();
                b.OwnsOne(e => e.WorkAddress);
            });

            // ModelBuilder API for value comparers
            //modelBuilder
            //    .Entity<EntityType>()
            //    .Property(e => e.MyProperty)
            //    .HasConversion(
            //        v => JsonSerializer.Serialize(v, null),
            //        v => JsonSerializer.Deserialize<List<int>>(v, null),
            //        new ValueComparer<List<int>>(
            //            (c1, c2) => c1.SequenceEqual(c2),
            //            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            //            c => c.ToList()))
            //;

            // Model building for fields
            // Finally for RC1, EF Core now allows use of the lambda methods in the ModelBuilder for fields as well as properties. For example, if you are averse to properties for some reason and decide to use public fields, then these fields can now be mapped using the lambda builders:
            modelBuilder.Entity<Blog>(b =>
            {
                b.Property(e => e.Id);
                b.Property(e => e.Name);
            });
            modelBuilder.Entity<Post>(b =>
            {
                b.Property(e => e.Id);
                b.Property(e => e.Name);
                b.Property(e => e.Category);
                b.Property(e => e.BlogId);
                b.HasOne(e => e.Blog).WithMany(e => e.Posts);
            });
        }
    }
}

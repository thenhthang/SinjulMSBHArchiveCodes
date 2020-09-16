using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;

using NET5RC1.Data;
using NET5RC1.SinjulMSBH;

namespace NET5RC1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGetAsync(
            [FromServices] ApplicationDbContext context,
            [FromServices] IMigrator migrator)
        {
            // Added Messagepack support in SignalR Java client

            //HubConnectionBuilder hb = new HubConnectionBuilder()
            //{

            //};

            //hb.Build();

            //HubConnection hubConnection =
            //    HubConnectionBuilder.create("https://localhost:5001/MyHub")
            //        .withHubProtocol(new MessagePackHubProtocol())
            //        .build()
            //;

            // Options for migration generation
            // Migrations scripts with transactions
            string gs = migrator.GenerateScript(
                default,
                default,
                default);
            await migrator.MigrateAsync(gs);

            // See pending migrations
            // dotnet ef migrations list
            // Get-Migration 

            // SaveChanges interception and events
            context.SavingChanges += (sender, args) =>
            {
                Console.WriteLine($"Saving changes for " +
                    $"{((DbContext)sender).Database.GetConnectionString()}");
            };
            context.SavedChanges += (sender, args) =>
            {
                Console.WriteLine($"Saved {args.EntitiesSavedCount} changes for " +
                    $"{((DbContext)sender).Database.GetConnectionString()}");
            };


            // Many to Many
            var beginnerTag = new Tag { Text = "Beginner" };
            var advancedTag = new Tag { Text = "Advanced" };
            var efCoreTag = new Tag { Text = "EF Core" };

            await context.AddRangeAsync(
                new Post { Name = "EF Core 101", Tags = new List<Tag> { beginnerTag, efCoreTag } },
                new Post { Name = "Writing an EF database provider", Tags = new List<Tag> { advancedTag, efCoreTag } },
                new Post { Name = "Savepoints in EF Core", Tags = new List<Tag> { beginnerTag, efCoreTag } });

            await context.SaveChangesAsync();

            var posts = await context.Posts.Include(e => e.Tags).ToListAsync();
            foreach (var post in posts)
            {
                Console.Write($"Post \"{post.Name}\" has tags");
                foreach (var tag in post.Tags)
                    Console.Write($" '{tag.Text}'");
            }


            // Map entity types to queries
            var posts2 = 
                await context.Posts
                    .Where(e => e.Blog.Name.Contains("Unicorn"))
                    .ToListAsync()
            ;

            // Property bags
            var beverages = new Dictionary<string, object>
            {
                ["Name"] = "Beverages",
                ["Description"] = "Stuff to sip on"
            };
            await context.Categories.AddAsync(beverages);
            await context.SaveChangesAsync();

            var foods = context.Categories.Single(e => e["Name"] == "Foods");
            var marmite = context.Products.Single(e => e["Name"] == "Marmite");
            marmite["CategoryId"] = foods["Id"];
            marmite["Description"] = "Yummy when spread _thinly_ on buttered Toast!";
            await context.SaveChangesAsync();


            // EntityEntry TryGetValue methods
            if (context.Entry(new Tag())
                .CurrentValues.TryGetValue<string>("Name", out var value))
            {
                Console.WriteLine(value);
            }

            // Default environment to Development
            // The EF Core command line tools now automatically configure the ASPNETCORE_ENVIRONMENT and DOTNET_ENVIRONMENT environment variables to "Development".
        }
    }
}

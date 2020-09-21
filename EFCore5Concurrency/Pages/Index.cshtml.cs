
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EFCore5Concurrency.Data;
using EFCore5Concurrency.SinjulMSBH;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFCore5Concurrency.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            ILogger<IndexModel> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;

            //InitialDatabase();
        }

        private void InitialDatabase()
        {
            if (!_context.Actors.Any())
            {
                _context.Actors.AddRange(new List<Actor> {
                    new Actor{
                        FirstName = "Sinjul" , LastName = "MSBH" , ContactNumber = "13-1313"
                    },
                    new Actor{
                        FirstName = "Jack" , LastName = "Slater" , ContactNumber = "17-1717"
                    },
                });

                _context.SaveChanges();
            }
        }

        public async Task OnGetAsync()
        {
            var actor =
                await _context.Actors.SingleOrDefaultAsync(p => p.ActorId == 1)
            ;
            actor.FirstName = "Jack4";

            await _context.Database.ExecuteSqlRawAsync
            (
                "UPDATE dbo.Actors SET FirstName = 'Jack8' WHERE ActorId = 1"
            );

            var savedData = false;

            while (!savedData)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    savedData = true;
                    _logger.LogInformation("Final FirstName: " + actor.FirstName);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is Actor)
                        {
                            var currentValues = entry.CurrentValues;
                            var dbValues = entry.GetDatabaseValues();

                            foreach (var prop in currentValues.Properties)
                            {
                                var currentValue = currentValues[prop];
                                var dbValue = dbValues[prop];

                                _logger.LogInformation(
                                    "currentValue: " + currentValue + "\tdbValue: " + dbValue
                                );
                            }

                            entry.OriginalValues.SetValues(dbValues);
                        }
                        else
                        {
                            throw new NotSupportedException
                            (
                                "Don’t know handling of conflict " + entry.Metadata.Name
                            );
                        }
                    }
                }
            }
        }
    }
}

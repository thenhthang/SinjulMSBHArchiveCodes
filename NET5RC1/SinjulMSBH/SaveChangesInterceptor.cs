using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NET5RC1.SinjulMSBH
{
    public class MySaveChangesInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            Console.WriteLine($"Saving changes for {eventData.Context.Database.GetConnectionString()}");

            return result;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"Saving changes asynchronously for {eventData.Context.Database.GetConnectionString()}");

            return new ValueTask<InterceptionResult<int>>(result);
        }
    }
}

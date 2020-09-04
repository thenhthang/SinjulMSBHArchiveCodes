using System;
using System.Threading.Tasks;

using Dotmim.Sync;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.SqlServer;
using Dotmim.Sync.Web.Client;

namespace Console_Dotmim_Sync
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // await SynchronizeAdventureWorksAsync();

            await SynchronizeClientSideAdventureWorksAsync();
        }

        private static async Task SynchronizeAdventureWorksAsync()
        {
            SqlSyncProvider serverProvider =
                new SqlSyncProvider(
                    "Server=SINJULMSBH\\MSSQLSERVERS2019;Database=AdventureWorks;Trusted_Connection=True"
                )
            ;

            SqliteSyncProvider clientProvider = new SqliteSyncProvider("advworks.db");

            SyncAgent agent = new SyncAgent(clientProvider, serverProvider, new string[] {
                "ProductCategory",
                "ProductModel",
                "Product",
                "Address",
                "Customer",
                "CustomerAddress",
                "SalesOrderHeader",
                "SalesOrderDetail"
            });

            // Hello Sync V2 use some nice features to get some feedbacks from your client database.
            // get some feedbacks from your client database.
            // As SynchronizeAsync() is an async method, we can use a IProgress<T> to get feedbacks during the active sync:
            SynchronousProgress<ProgressArgs> progress =
                new SynchronousProgress<ProgressArgs>(s =>
                    Console.WriteLine($"{s.Context.SyncStage}:\t{s.Message}")
            );

            do
            {
                Console.Clear();
                Console.WriteLine("Sync Start");
                try
                {
                    SyncResult syncContext = await agent.SynchronizeAsync(progress);
                    Console.WriteLine(syncContext);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            } while (Console.ReadKey().Key != ConsoleKey.Escape);
        }

        private static async Task SynchronizeClientSideAdventureWorksAsync()
        {
            WebClientOrchestrator proxyClientProvider =
                new WebClientOrchestrator("https://localhost:5001/api/sync")
            ;

            SqliteSyncProvider clientProvider =
                new SqliteSyncProvider("advworksWithAPI.db");

            // Using the IProgress<T> pattern to handle progession dring the synchronization
            SynchronousProgress<ProgressArgs> progress =
                new SynchronousProgress<ProgressArgs>(s =>
                    Console.WriteLine($"{s.Context.SyncStage}:\t{s.Message}")
            );

            // Creating an agent that will handle all the process
            SyncAgent agent = new SyncAgent(clientProvider, proxyClientProvider);

            do
            {
                // Launch the sync process
                SyncResult s1 = await agent.SynchronizeAsync(progress);
                // Write results
                Console.WriteLine(s1);

            } while (Console.ReadKey().Key != ConsoleKey.Escape);

            Console.WriteLine("End");
        }
    }
}

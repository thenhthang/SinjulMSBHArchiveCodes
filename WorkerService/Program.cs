using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WorkerService
{
    public class Program
    {
        public static Task Main(string[] args) => CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    var configuration = hostContext.Configuration;
                    var provider = configuration.GetValue("Provider", "SqlServer");

                    services.AddPooledDbContextFactory<BlogContext>(
                        options => _ = provider switch
                        {
                            "Sqlite" => options.UseSqlite(
                                configuration.GetConnectionString("SqliteConnection"),
                                _ =>
                                {
                                    _.MigrationsAssembly("SqliteMigrations");
                                    _.MigrationsHistoryTable("__MyMigrationsHistory", "mySchema");
                                }
                            )
                            .EnableDetailedErrors()
                            .EnableSensitiveDataLogging()
                            //.ReplaceService<IMigrationsSqlGenerator, MyMigrationsSqlGenerator>()
                            .ReplaceService<IHistoryRepository, MyHistoryRepository>()
                            ,

                            "SqlServer" => options.UseSqlServer(
                                configuration.GetConnectionString("SqlServerConnection"),
                                _ =>
                                {
                                    _.MigrationsAssembly("SqlServerMigrations");
                                    _.MigrationsHistoryTable("__MyMigrationsHistory", "mySchema");
                                }
                            )
                            .EnableDetailedErrors()
                            .EnableSensitiveDataLogging()
                            //.ReplaceService<IMigrationsSqlGenerator, MyMigrationsSqlGenerator>()
                            .ReplaceService<IHistoryRepository, MyHistoryRepository>()
                            ,

                            _ => throw new Exception($"Unsupported provider: {provider}")
                        },42);
                });
    }
}

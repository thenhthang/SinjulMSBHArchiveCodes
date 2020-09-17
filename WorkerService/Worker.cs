using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerService
{
    public class Worker : BackgroundService, IAsyncDisposable
    {
        public Worker(ILogger<Worker> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _provider;

        Timer _timer;

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(
                async state => await ExecuteAsync(cancellationToken),
                state: null,
                dueTime: TimeSpan.Zero,
                period: TimeSpan.FromDays(1)
            );

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public override void Dispose() => _timer?.Dispose();

        public ValueTask DisposeAsync() => _timer.DisposeAsync();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _provider.CreateScope();
            var services = scope.ServiceProvider;

            var contextFactory = services.GetRequiredService<IDbContextFactory<BlogContext>>();
            using var context = contextFactory.CreateDbContext();

            var sql = context.Database.GenerateCreateScript();
            _logger.LogInformation(sql);

            // Initialize the schema for this DbContext
            var databaseCreator = services.GetService<IRelationalDatabaseCreator>();
            //await databaseCreator.CreateTablesAsync();

            var migrator = services.GetService<IMigrator>();
            //var generateScript = migrator.GenerateScript(
            //    "InitialCreate",
            //    "RemovePersonIndex",
            //    MigrationsSqlGenerationOptions.Default)
            //;
            //_logger.LogInformation(generateScript);
            //await migrator.MigrateAsync(default, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

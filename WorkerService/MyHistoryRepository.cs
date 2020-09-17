
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Sqlite.Migrations.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;

namespace WorkerService
{
    public class MyHistoryRepository : SqlServerHistoryRepository
    {
        public MyHistoryRepository(HistoryRepositoryDependencies dependencies) 
            : base(dependencies)
        {
        }

        protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
        {
            base.ConfigureTable(history);

            history.Property(h => h.MigrationId).HasColumnName("Id");
        }
    }
}

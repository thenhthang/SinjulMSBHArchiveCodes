using System;
using System.IO;

using Dotmim.Sync;
using Dotmim.Sync.SqlServer;

using Dotmim_Sync.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dotmim_Sync
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();

            services.AddControllers();

            // [Required] Mandatory to be able to handle multiple sessions
            services.AddMemoryCache();

            // [Required] Get a connection string for your server data source
            string connectionString =
                Configuration.GetSection("ConnectionStrings")["DefaultConnection"];

            // [Optional] Set the web server Options || WebServerOptions
            SyncOptions options = new SyncOptions()
            {
                BatchDirectory =
                    Path.Combine(
                        SyncOptions.GetDefaultUserBatchDiretory(),
                        "server"
                    )
                ,
                SnapshotsDirectory =
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        "Snapshots"
                    )
            };

            // [Required] Create the setup used for your sync process
            string[] tables = new string[] {
                "ProductCategory", "ProductModel", "Product",
                "Address", "Customer", "CustomerAddress",
                "SalesOrderHeader", "SalesOrderDetail"
            };

            // [Optional] Defines the schema prefix and suffix for all generated objects
            SyncSetup setup = new SyncSetup(tables)
            {
                // optional :
                StoredProceduresPrefix = "server",
                StoredProceduresSuffix = "",
                TrackingTablesPrefix = "server",
                TrackingTablesSuffix = ""
            };


            // [Required] add a SqlSyncProvider acting as the server hub
            services.AddSyncServer<SqlSyncChangeTrackingProvider>(
                connectionString, setup, options
            );
            //services.AddSyncServer<SqlSyncProvider>(connectionString, setup, options);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}

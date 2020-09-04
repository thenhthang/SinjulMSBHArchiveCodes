using System.Threading.Tasks;

using Dotmim.Sync.Web.Server;

using Microsoft.AspNetCore.Mvc;

namespace Dotmim_Sync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        // The WebServerManager instance is useful to manage all
        // the Web server orchestrators registered in the Startup.cs
        private readonly WebServerManager _manager;

        // Injected thanks to Dependency Injection
        public SyncController(WebServerManager manager) => _manager = manager;

        /// <summary>
        /// This POST handler is mandatory to handle all the sync process
        /// </summary>
        [HttpPost]
        public async Task Post() => await _manager.HandleRequestAsync(HttpContext);

        /// <summary>
        /// This GET handler is optional. It allows you to see the configuration hosted on the server
        /// The configuration is shown only if Environmenent == Development
        /// </summary>
        [HttpGet]
        public async Task Get() => await _manager.HandleRequestAsync(HttpContext);






        //private WebServerOrchestrator _webProxyServer;

        //// Injected thanks to Dependency Injection
        //public SyncController(WebServerOrchestrator proxy) => _webProxyServer = proxy;

        ///// <summary>
        ///// [Optional] This Get request is just here to show a default page
        ///// </summary>
        //[HttpGet]
        //public async Task Get() =>
        //    await _webProxyServer.HandleRequestAsync(HttpContext, _ => { });

        ///// <summary>
        ///// [Required] This Post request is called by the all Dotmim.Sync.Web.Client apis
        ///// </summary>
        //[HttpPost]
        //public async Task Post() =>
        //    await _webProxyServer.HandleRequestAsync(HttpContext, _ => { });

        //[HttpPost]
        //public async Task Do()
        //{
        //    var interceptor = new Interceptor<SchemaArgs>(args =>
        //    {
        //        IList<string> neededTables = Tables.GetNeededTables();
        //        var tablesToRemove = new List<DmTable>();
        //        var schema = _webProxyServerProvider.GetLocalProvider(HttpContext).Configuration.Schema;
        //        // remove unnecessary tables
        //        foreach (var dmTable in schema.Tables)
        //        {
        //            var any = neededTables.Any(table1 => table1 == dmTable.TableName);
        //            if (any) continue;

        //            foreach (var relation in dmTable.ParentRelations)
        //            {
        //                schema.Relations.Remove(relation);
        //            }

        //            foreach (var relation in dmTable.ChildRelations)
        //            {
        //                schema.Relations.Remove(relation);
        //            }

        //            tablesToRemove.Add(dmTable);
        //        }

        //        foreach (var dmTable in tablesToRemove)
        //        {
        //            schema.Tables.Remove(dmTable);
        //        }
        //    });

        //    _webProxyServer.GetLocalProvider(HttpContext).On(interceptor);

        //    await _webProxyServer.HandleRequestAsync(HttpContext);
        //}
    }
}

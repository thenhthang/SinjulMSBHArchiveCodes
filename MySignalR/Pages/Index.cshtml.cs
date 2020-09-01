using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

using MySignalR.Hubs;

namespace MySignalR.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<IndexModel> _logger;

        private readonly IHubContext<ChatHubPoco> _hubContext;

        public IHubContext<ChatHubFull, IChatClient> _strongChatHubContext { get; }

        public IndexModel(
            IWebHostEnvironment webHostEnvironment,
            ILogger<IndexModel> logger,
            IHubContext<ChatHubPoco> hubContext,
            IHubContext<ChatHubFull, IChatClient> chatHubContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _hubContext = hubContext;
            _strongChatHubContext = chatHubContext;
        }

        public async Task OnGetAsync()
        {
            await _hubContext.Clients.All.SendAsync("SendMessage", "user", "message");
        }

        public async Task SendMessage(string message)
        {
            await _strongChatHubContext.Clients.All.ReceiveMessage(message);
        }



        [BindProperty, Display(Name = "File")]
        public IFormFile UploadedFile { get; set; }
        public async Task<JsonResult> OnPostAsync()
        {
            string fileName =
                ContentDispositionHeaderValue.Parse(UploadedFile.ContentDisposition)
                .FileName.Trim('"')
            ;

            fileName = EnsureCorrectFilename(fileName);

            using FileStream output = System.IO.File.Create(GetPathAndFilename(fileName));

            await UploadedFile.CopyToAsync(output);

            return new JsonResult(new { path = GetPath(), fileName });
        }

        private static string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

        private string GetPath() =>
            $"{_webHostEnvironment.WebRootPath}\\uploads\\";

        private string GetPathAndFilename(string filename) =>
            $"{_webHostEnvironment.WebRootPath}\\uploads\\{filename}";
    }
}

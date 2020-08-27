using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using DotNET5Preview8.SinjulMSBH;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace DotNET5Preview8.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public IndexModel(ILogger<IndexModel> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        [BindProperty] public Person Person { get; set; }


        public PageResult OnPost(
            [Required] string name = "SinjulMSBH",
            [Range(0, 150)] byte age = 28)
        {
            Person = new()
            {
                Age = age,
                Name = name
            };

            _logger.LogInformation(
                new EventId(13131313, HttpContext.TraceIdentifier),
                $"Name: {Person.Name} and Age: {Person.Age} .. !!!!"
            );

            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder

                    .ClearProviders()

                    .AddJsonConsole(options =>
                    {
                        options.JsonWriterOptions = new JsonWriterOptions
                        {
                            Indented = true,
                            SkipValidation = false,
                        };
                    })
                    // current usage which is to be changed:
                    //.AddConsole(o =>
                    //{
                    //    o.Format = ConsoleLoggerFormat.Default;  //< ---deprecated
                    //    o.IncludeScopes = true;                  //< ---deprecated
                    //    o.UseUtcTimestamp = true;                //< ---deprecated
                    //    o.TimestampFormat = "HH:mm:ss";          //< ---deprecated
                    //    o.DisableColors = false;                 //< ---deprecated
                    //    o.FormatterName = "ConsoleLogFormatterNames.Default";
                    //    o.LogToStandardErrorThreshold = LogLevel.Trace;
                    //})
                    ;
            });

            ILogger<IndexModel> logger = loggerFactory.CreateLogger<IndexModel>();

            logger.LogInformation(
                new EventId(17171717, HttpContext.TraceIdentifier),
                $"Name: {Person.Name} and Age: {Person.Age} .. !!!!"
            );

            return Page();
        }
    }
}

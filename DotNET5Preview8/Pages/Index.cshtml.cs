using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using DotNET5Preview8.SinjulMSBH;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DotNET5Preview8.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger) => _logger = logger;


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

            return Page();
        }
    }
}

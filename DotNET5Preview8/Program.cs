
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.Json;

using DotNET5Preview8;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Microsoft.Extensions.Logging;


Console.WriteLine("Hello World!");

FromWhom();

Show.Excitement("Top-level programs can be brief, and can grow as slowly or quickly in complexity as you'd like", 8);

void FromWhom() => Console.WriteLine($"From {RuntimeInformation.FrameworkDescription}");


//Target - typed new expressions
List<string> values = new List<string>();
List<string> values2 = new();
var values3 = new List<string>();

//Pattern matching
ContextTest context = new();
if (context is { IsReachable: false, Length: > 1 })
    Console.WriteLine(context.Name);




CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
            logging.AddJsonConsole(options =>
            {
                options.JsonWriterOptions = new JsonWriterOptions() {
                    Indented = true,
                    SkipValidation = false,
                };
            });
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });

internal class Show
{
    internal static void Excitement(string message, int levelOf)
    {
        Console.Write(message);

        for (int i = 0; i < levelOf; i++) Console.Write("!");

        Console.WriteLine();
    }
}

internal class ContextTest
{
    public bool IsReachable { get; set; }
    public int Length { get; set; } = 13;
    public string Name { get; set; } = "SinjulMSBH";

}
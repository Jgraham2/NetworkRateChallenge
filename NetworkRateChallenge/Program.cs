using Azure.Storage.Blobs;
using CodingChallenge.Models;
using CodingChallenge.Services.Azure;
using CodingChallenge.Services.Rates;
using CodingChallenge.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;

namespace CodingChallenge;

internal class Program
{
    private static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        var rateService = host.Services.GetRequiredService<RateService>();

        rateService.CalculateRates();

        host.Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<AzureSettings>(hostContext.Configuration.GetSection("AzureSettings"));
                services.AddTransient<RateService>();
                services.AddTransient<AzureService>();
                services.AddTransient<IFileUtils, FileUtils>();
            });
}
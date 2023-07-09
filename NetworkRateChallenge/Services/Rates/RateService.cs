using CodingChallenge.Models;
using CodingChallenge.Services.Azure;
using CodingChallenge.Utils;
using Microsoft.Extensions.Configuration;

namespace CodingChallenge.Services.Rates
{
    public class RateService
    {
        private readonly AzureService _azureService;
        private readonly AzureSettings _azureSettings;
        private readonly IFileUtils _fileUtils;

        public RateService(AzureService azureService, IConfiguration configuration, IFileUtils fileUtils)
        {
            _azureService = azureService;
            _azureSettings = new AzureSettings();
            _fileUtils = fileUtils;
            configuration.GetSection("AzureSettings").Bind(_azureSettings);
        }

        public void CalculateRates()
        {

            var connectionString = _azureSettings.ConnectionString;
            var containerName = _azureSettings.ContainerName;
            var graphFile = _azureSettings.GraphFile;
            var ratesFile = _azureSettings.RatesFile;

            var dotFilePath = _azureService.DownloadDotFileFromAzureBlob(connectionString, containerName, graphFile);

            var dotFileContent = File.ReadAllText(dotFilePath);
            var json = _fileUtils.ConvertDotFileToJson(dotFileContent);

            Console.WriteLine($"Network Measurements Are:--");
            Console.WriteLine(json);
            Console.WriteLine();

            var rateCardData = _azureService.ReadJsonFromAzureBlob(connectionString, containerName, ratesFile);

            var rateCards = RateCalculator.ParseRates(rateCardData);

            Console.WriteLine();
            Console.WriteLine($"Current Rate Card is:--");

            Console.WriteLine();
            Console.WriteLine(rateCards);

            var rateCardTotals = RateCalculator.CalculateRateCardTotals(rateCardData, json);
            foreach (var rateCard in rateCardTotals)
            {
                Console.WriteLine($"Rate Card: {rateCard.Key}, Total Cost: {rateCard.Value:C}");
            }
        }
    }
}

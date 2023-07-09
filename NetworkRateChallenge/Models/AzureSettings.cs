using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CodingChallenge.Models
{
    public class AzureSettings
    {
        public string? ConnectionString { get; set; }
        public string? ContainerName { get; set; }
        public string? GraphFile { get; set; }
        public string? RatesFile { get; set; }
    }

    public class AzureSettingsOptions : IConfigureOptions<AzureSettings>
    {
        private readonly IConfiguration _configuration;

        public AzureSettingsOptions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(AzureSettings azureSettings)
        {
            _configuration.GetSection("AzureSettings").Bind(azureSettings);
        }
    }
}
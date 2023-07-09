using Azure.Storage.Blobs;
using CodingChallenge.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace CodingChallenge.Services.Azure
{
    public class AzureService
    {
        private readonly AzureSettings _azureSettings;

        public AzureService(IOptions<AzureSettings> azureSettingsOptions)
        {
            _azureSettings = azureSettingsOptions.Value ?? throw new ArgumentNullException(nameof(azureSettingsOptions));
        }

        public JObject ReadJsonFromAzureBlob(string? connectionString, string? containerName, string? blobName)
        {
            var containerClient = new BlobContainerClient(connectionString, containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            using var memoryStream = new MemoryStream();
            blobClient.DownloadTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(memoryStream);
            string json = reader.ReadToEnd();
            return JObject.Parse(json);
        }

        public string DownloadDotFileFromAzureBlob(string? connectionString, string? containerName, string? blobName)
        {
            var dotFilePath = Path.GetTempFileName();

            var containerClient = new BlobContainerClient(connectionString, containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            using var dotFileStream = File.OpenWrite(dotFilePath);
            blobClient.DownloadTo(dotFileStream);

            return dotFilePath;
        }
    }
}

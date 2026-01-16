using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Kebabify.Api.Common;

using Microsoft.Extensions.Logging;

namespace Kebabify.Api.Services
{
    public class StorageService(BlobServiceClient blobServiceClient, ILogger<StorageService> logger) : IStorageService
    {
        private const string Container = "kebabify";

        public async Task Persist(string input, string result)
        {
            logger.LogInformation("Create blobcontainerClient for: '{Container}'", Container);

            var containerClient = blobServiceClient.GetBlobContainerClient(Container);
            await containerClient.CreateIfNotExistsAsync();

            logger.LogInformation("Create blobClient");
            var blobClient = containerClient.GetBlobClient(GenereateFilename());

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = "application/json" }
            };

            var item = new Item(input, result);

            logger.LogInformation("Persist file to storage");
            await blobClient.UploadAsync(BinaryData.FromString(item.ToJson()), options: options);
        }

        private static string GenereateFilename()
        {
            return $"{Guid.NewGuid()}.json";
        }

        private sealed record Item(string Input, string Result);
    }
}

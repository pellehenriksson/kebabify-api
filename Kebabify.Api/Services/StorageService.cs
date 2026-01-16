using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Kebabify.Api.Common;

using Microsoft.Extensions.Logging;

namespace Kebabify.Api.Services
{
    public class StorageService : IStorageService
    {
        private readonly BlobServiceClient blobServiceClient;

        private readonly ILogger<StorageService> logger;

        public StorageService(BlobServiceClient blobServiceClient, ILogger<StorageService> logger)
        {
            this.blobServiceClient = blobServiceClient;
            this.logger = logger;
        }

        public async Task Persist(string input, string result)
        {
            try
            {
                var containerClient = this.blobServiceClient.GetBlobContainerClient("kebabify");
                await containerClient.CreateIfNotExistsAsync();

                var blobClient = containerClient.GetBlobClient(GenereateFilename());

                var options = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = "application/json" }
                };

                var item = new Item(input, result);

                await blobClient.UploadAsync(BinaryData.FromString(item.ToJson()), options: options);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static string GenereateFilename()
        {
            return $"{Guid.NewGuid()}.json";
        }

        private sealed record Item(string Input, string Result);

    }
}

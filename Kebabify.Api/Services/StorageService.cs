using Azure.Storage.Blobs;

namespace Kebabify.Api.Services
{
    public class StorageService : IStorageService
    {

        private readonly BlobServiceClient blobServiceClient;

        public StorageService(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }
    }
}

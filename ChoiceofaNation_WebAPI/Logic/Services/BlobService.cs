using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;

namespace ChoiceofaNation_WebAPI.Logic.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobService(IConfiguration configuration)
        {
            var storageConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new InvalidOperationException("Azure Storage connection string is missing.");
            }

            _blobServiceClient = new BlobServiceClient(storageConnectionString);
            _containerName = configuration["AzureStorage:ContainerName"];
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);

            return blobClient.Uri.ToString();
        }
    }
}

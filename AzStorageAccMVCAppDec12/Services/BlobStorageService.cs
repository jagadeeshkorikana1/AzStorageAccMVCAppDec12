using AzStorageAccMVCAppDec12.Models;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzStorageAccMVCAppDec12.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        public BlobStorageService()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["AzureStorageConnectionString"].ConnectionString;
            var containerName = ConfigurationManager.AppSettings["BlobContainerName"];
            var serviceClient = new BlobServiceClient(connectionString);
            _containerClient = serviceClient.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists();
        }
        public IEnumerable<BlobItemViewModel> ListBlobs()
        {
            var items = new List<BlobItemViewModel>();
            foreach (BlobItem blobItem in _containerClient.GetBlobs())
            {
                var blobClient = _containerClient.GetBlobClient(blobItem.Name);
                items.Add(new BlobItemViewModel
                {
                    Name = blobItem.Name,
                    Uri = blobClient.Uri,
                    Size = blobItem.Properties.ContentLength,
                    ContentType = blobItem.Properties.ContentType,
                    LastModified = blobItem.Properties.LastModified
                });
            }
            return items.OrderBy(i => i.Name);
        }
        public async Task UploadAsync(string blobName, Stream content, string contentType)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            var headers = new BlobHttpHeaders { ContentType = contentType ?? "application/octet-stream" };
            await blobClient.UploadAsync(content, new BlobUploadOptions { HttpHeaders = headers });
        }
        public async Task<Stream> DownloadAsync(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            var ms = new MemoryStream();
            await blobClient.DownloadToAsync(ms);
            ms.Position = 0;
            return ms;
        }
        public async Task<bool> DeleteAsync(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            Response<bool> response = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            return response.Value;
        }
    }
}
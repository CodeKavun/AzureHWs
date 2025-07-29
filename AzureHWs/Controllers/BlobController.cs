using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace AzureHWs.Controllers
{
    public class BlobController : Controller
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly string containerName;

        public BlobController(IConfiguration configuration, BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
            containerName = configuration["AzureBlobStorage:ContainerName"]!;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("No file selected.");

            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(file.FileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return View();
        }
    }
}

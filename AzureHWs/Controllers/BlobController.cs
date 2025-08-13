using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;

namespace AzureHWs.Controllers
{
    public class BlobController : Controller
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly QueueServiceClient queueServiceClient;

        private readonly string containerName;
        private readonly string queueName;

        public BlobController(IConfiguration configuration, BlobServiceClient blobServiceClient, QueueServiceClient queueServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
            this.queueServiceClient = queueServiceClient;

            containerName = configuration["AzureBlobStorage:ContainerName"]!;
            queueName = configuration["AzureBlobStorage:QueueName"]!;
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

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(file.FileName);

            using Stream stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            await SendFileNameToQueue(file);

            return View();
        }

        private async Task SendFileNameToQueue(IFormFile file)
        {
            QueueClient client = queueServiceClient.GetQueueClient(queueName);
            await client.CreateIfNotExistsAsync();

            await client.SendMessageAsync(file.FileName);
        }
    }
}

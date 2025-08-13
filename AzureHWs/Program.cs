using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

var builder = WebApplication.CreateBuilder(args);

var blobConnectionString = builder.Configuration["AzureBlobStorage:ConnectionString"];
builder.Services.AddSingleton(new BlobServiceClient(blobConnectionString));
builder.Services.AddSingleton(new QueueServiceClient(blobConnectionString));

builder.Services.AddControllersWithViews();

string containerName = builder.Configuration["AzureBlobStorage:ContainerName"]!;
string smallImagesContainer = builder.Configuration["AzureBlobStorage:CompressedImagesContainer"]!;
string queueName = builder.Configuration["AzureBlobStorage:QueueName"]!;

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.UseStaticFiles();
app.UseDefaultFiles();

app.MapControllerRoute("default", "{controller=Blob}/{action=Upload}/{id?}");

// Receive messages and send compressed images to appropriate container
BlobContainerClient containerClient = app.Services.GetService<BlobServiceClient>()!.GetBlobContainerClient(containerName);
BlobContainerClient smallImagesContainerClient = app.Services.GetService<BlobServiceClient>()!.GetBlobContainerClient(smallImagesContainer);
QueueClient queueClient = app.Services.GetService<QueueServiceClient>()!.GetQueueClient(queueName);

foreach (PeekedMessage peekedMessage in (await queueClient.PeekMessagesAsync(maxMessages: 3)).Value)
{
    string fileName = peekedMessage.Body.ToString();

    BlobClient blobClient = containerClient.GetBlobClient(fileName);
    BlobClient smallImagesBlobClient = smallImagesContainerClient.GetBlobClient(fileName);

    if (!await smallImagesBlobClient.ExistsAsync())
    {
        var downloadInfo = await blobClient.DownloadStreamingAsync();

        Stream gainedImage = downloadInfo.Value.Content;

        using Image image = Image.Load(gainedImage);
        int width = image.Width / 2;
        int height = image.Height / 2;
        image.Mutate(x => x.Resize(width, height));

        using MemoryStream ms = new MemoryStream();
        await image.SaveAsync(ms, new PngEncoder());
        ms.Position = 0;

        await smallImagesBlobClient.UploadAsync(ms);
    }
}

// ---------------------

app.Run();

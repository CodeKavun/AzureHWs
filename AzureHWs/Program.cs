using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

var blobConnectionString = builder.Configuration["AzureBlobStorage:ConnectionString"];
builder.Services.AddSingleton(new BlobServiceClient(blobConnectionString));

builder.Services.AddControllersWithViews();

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.UseStaticFiles();
app.UseDefaultFiles();

app.MapControllerRoute("default", "{controller=Blob}/{action=Upload}/{id?}");

app.Run();

// See https://aka.ms/new-console-template for more information
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

Console.WriteLine("Azure Storage and Blob Container!\n");

//required in case of created variable in users' OS Windows
//string? connectionString = Environment.GetEnvironmentVariable("CONNECT_STR");

//copy connection string from section Access Key of Storage account
string connectionString = "";

//connection to azure storage
BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
Console.WriteLine($"stotage:\t{blobServiceClient.AccountName}");

//creation of container on azure storage
string containerName = "files";
BlobContainerClient containerClient = null!;
//containerClient = blobServiceClient.CreateBlobContainer(containerName);

//get blob containerClient by its' name
containerClient = blobServiceClient.GetBlobContainerClient(containerName);
Console.WriteLine($"blob container:\t{containerClient.Name}\n");

//get blobClient instance for download and upload files (FIO) in blob container
string blobItemName = "documents";
BlobClient blobClient = containerClient.GetBlobClient(blobItemName);

//get list of blobItems
Pageable<BlobItem> blobItems = containerClient.GetBlobs();
foreach (BlobItem item in blobItems)
{
    if (item.Deleted)
        continue;
    Console.WriteLine($"BlobItem:\t{item.Name}");
}
BlobItem? blobItem = blobItems.FirstOrDefault(b => b.Equals("files"));

//get list of blob containers from Azure storage
Pageable<BlobContainerItem> containerItems = blobServiceClient.GetBlobContainers();
foreach (BlobContainerItem item in containerItems)
{
    if(item.IsDeleted != null) 
        continue;
    Console.WriteLine($"BloBContainerItem:\t{item.Name}");
}

Console.WriteLine("\nend");
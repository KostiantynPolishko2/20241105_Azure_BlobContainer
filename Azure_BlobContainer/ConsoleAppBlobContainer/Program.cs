// See https://aka.ms/new-console-template for more information
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

Console.WriteLine("Azure Storage and Blob Container!\n");

//required in case of created variable in users' OS Windows
//string? connectionString = Environment.GetEnvironmentVariable("CONNECT_STR");

//copy connection string from section Access Key of Storage account
string connectionString = "";

//connection to Azure storage
BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
Console.WriteLine($"storage:\t{blobServiceClient.AccountName}");


//=================================LIST OF CONTAINERS=================================//

//get list of blobContainers from Azure storage
Pageable<BlobContainerItem> containerItems = blobServiceClient.GetBlobContainers();
Console.WriteLine("\nBloBContainerItems:");
foreach (BlobContainerItem item in containerItems)
{
    if (item.IsDeleted != null)
        continue;
    Console.WriteLine($"\t{item.Name}");
}

//creation of container on azure storage
string containerName = "files";
//BlobContainerClient blobContainerClient = null!;
//containerClient = blobServiceClient.CreateBlobContainer(containerName);

//get list of blobItems (files inside blob container)
Pageable<BlobItem> blobItems = blobServiceClient.GetBlobContainerClient(containerName).GetBlobs();
Console.WriteLine("\nBlobItem:");
foreach (BlobItem item in blobItems)
{
    if (item.Deleted)
        continue;
    Console.WriteLine($"\t{item.Name}");
}
BlobItem? blobItem = blobItems.FirstOrDefault(b => b.Equals("files"));

//=================================FILE FIO (DOWNLOAD / UPLOAD)=================================//

//get blob containerClient by its' name
string blobContainer = "files";
BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainer);
Console.WriteLine($"\nblob container:\t{blobContainerClient.Name}");

//get blobClient instance for download and upload files (FIO) in blob container
foreach(var item in blobItems)
{
    if (item.Deleted)
        continue;
    else if (item.Name.Contains("/"))
        continue;

    //get client over file in container
    BlobClient blobClient = blobContainerClient.GetBlobClient(item.Name);
    //create new path of filestream for download selected file
    string downloadPath = $"../../../AzureBlobs/{item.Name}";

    try
    {
        using (FileStream fileStream = File.OpenWrite(downloadPath))
        {
            blobClient.DownloadTo(fileStream);
        }
        Console.WriteLine($"downloded file:\t{item.Name}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{item.Name} did not download");
    }

}

//example to download one file
//
//string blobItemName = blobItems.ToArray()[1].Name;
//BlobClient blobClient = blobContainerClient.GetBlobClient(blobItemName);

//string downloadFilePath = $"../../../AzureBlobs/downlod_{blobItemName}";
//FileStream fileStream = File.OpenWrite(downloadFilePath);
//blobClient.DownloadTo(fileStream);

//blobClient.Delete(DeleteSnapshotsOption.IncludeSnapshots);
//Console.WriteLine($"downloded file:\t{blobItemName}");
//fileStream.Close();

Console.WriteLine("\nend");
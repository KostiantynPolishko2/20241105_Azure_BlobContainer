// See https://aka.ms/new-console-template for more information
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage;

Console.WriteLine("Azure Storage and Blob Container!\n");

//required in case of created variable in users' OS Windows
//copy connection string from section Access Key of Storage account
//string? connectionString = Environment.GetEnvironmentVariable("CONNECT_STR");

//string accountName = "PolXs_wp31@student.itstep.org";
string storageName = "docfiles";
string accountKey1 = "";

Uri serviceUri = new Uri($"https://{storageName}.blob.core.windows.net");

//connection to Azure storage
//BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

StorageSharedKeyCredential credential = new StorageSharedKeyCredential(storageName, accountKey1);
BlobServiceClient blobServiceClient = new BlobServiceClient(serviceUri, credential);
Console.WriteLine($"storage:\t{blobServiceClient.AccountName}");

//=================================LIST OF CONTAINERS=================================//
//
//++++++get list of blobContainers from Azure storage++++++//
//
Pageable<BlobContainerItem> blobContainers = blobServiceClient.GetBlobContainers();
Console.WriteLine("\nBloBContainerItems:");
foreach (BlobContainerItem item in blobContainers)
{
    if (item.IsDeleted != null)
        continue;
    Console.WriteLine($"\t{item.Name}");
}

//++++++creation of container on azure storage++++++//
//
string blobItemName = "files";
//BlobContainerClient blobContainerClient = null!;
//containerClient = blobServiceClient.CreateBlobContainer(blobItemName);

//++++++get list of blobItems (files inside blob container)++++++//
Pageable<BlobItem> blobItems = blobServiceClient.GetBlobContainerClient(blobItemName).GetBlobs();
Console.WriteLine("\nBlobItem:");
foreach (BlobItem item in blobItems)
{
    if (item.Deleted)
        continue;
    Console.WriteLine($"\t{item.Name}");
}
BlobItem? blobItem = blobItems.FirstOrDefault(b => b.Equals("files"));

//=================================FILE FIO (DOWNLOAD / UPLOAD)=================================//
//
//++++++get blob containerClient by its' name++++++//
//
string blobContainer = "files";
BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainer);
Console.WriteLine($"\nblob container:\t{blobContainerClient.Name}");

//canceletionToken for manage time of downoloading
CancellationTokenSource cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromSeconds(5));

//++++++get blobClient instance for download files in blob container++++++//
//
foreach (var item in blobItems)
{
    if (item.Deleted)
        continue;
    else if (item.Name.Contains("image/"))
    {
        BlobClient blobClient2 = blobContainerClient.GetBlobClient(item.Name);

        //using BlobDownloadresult will be not blocked container for another operations
        BlobDownloadResult downloadInfo = blobClient2.DownloadContent();
        string downloadPath2 = $"../../../AzureBlobs/{item.Name.Replace("image/","")}";
        using (FileStream fileStream2 = File.OpenWrite(downloadPath2))
        {
            await downloadInfo.Content.ToStream().CopyToAsync(fileStream2, cts.Token);
        }
        continue;
    }

    //get client over file in container
    BlobClient blobClient = blobContainerClient.GetBlobClient(item.Name);
    //create new path of filestream for download selected file
    string downloadPath = $"../../../AzureBlobs/{item.Name}";

    try
    {
        using (FileStream fileStream = File.OpenWrite(downloadPath))
        {
            blobClient.DownloadTo(fileStream, cts.Token);
        }
        Console.WriteLine($"downloded file:\t{item.Name}");

        //blobClient.Delete(DeleteSnapshotsOption.IncludeSnapshots);
        //Console.WriteLine($"deleted file:\t{item.Name}");
    }
    catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.ContainerDisabled || ex.ErrorCode == BlobErrorCode.ContainerBeingDeleted)
    {
        Console.WriteLine($"BlobItem {item.Name} is absent");
    }
    catch (RequestFailedException ex)
    {
        Console.WriteLine($"unexpected ererror: {ex.Message}, code {ex.ErrorCode}");
    }
    finally
    {
        Console.WriteLine("THE END\n");
    }
}

//++++++example to upload one file to Blob Container++++++//
//
//string fileName = "test2.txt";
//string downloadFilePath = $"../../../AzureBlobs/{fileName}";

//try
//{
//    using (FileStream fileStreamUp = File.OpenRead(downloadFilePath))
//    {
//        blobContainerClient.UploadBlob(fileName, fileStreamUp);
//    }
//}
//catch (RequestFailedException ex)
//{
//    Console.WriteLine($"uploading file {fileName} is failed.\nErrorCode: {ex.ErrorCode}, Status: {ex.Status}");
//}

//CloudStorageAccount cloudStorageAccount = null;
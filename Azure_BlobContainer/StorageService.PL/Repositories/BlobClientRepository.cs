using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using StorageService.PL.Entities;
using StorageService.PL.Infrastructures;
using StorageService.PL.Interfaces;

namespace StorageService.PL.Repositories
{
    public class BlobClientRepository:IBlobClientRepository
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly string relativePath;

        public BlobClientRepository(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
            this.relativePath = @"AzureDownloads/";
        }

        private void isBlobServiceClient()
        {
            if (this.blobServiceClient == null)
                throw new DirectoryNotFoundException("BlobServiceClient is null");
        }

        public string getStorageName()
        {
            isBlobServiceClient();

            return this.blobServiceClient.AccountName;
        }

        public IEnumerable<string> getContainerNames()
        {
            isBlobServiceClient();

            List<string> names = new List<string>();
            Pageable<BlobContainerItem> containerItems = this.blobServiceClient.GetBlobContainers();

            foreach (BlobContainerItem containerItem in containerItems)
            {
                if (containerItem.IsDeleted != null)
                    continue;
                names.Add(containerItem.Name.ToLower());
            }

            if (names.Count() == 0)
                throw new BlobClientException("no containers", "name");

            return names;
        }

        public IEnumerable<UserBlobItem> getBlobItemNames(string containerName)
        {
            Pageable<BlobItem>? allBlobItems = this.blobServiceClient.GetBlobContainerClient(containerName)?.GetBlobs();

            if (allBlobItems == null)
                throw new BlobClientException("no data", "BlobItem");

            List<UserBlobItem> blobItems = new List<UserBlobItem>();
            foreach(BlobItem blobItem in allBlobItems)
            {
                if(blobItem.Deleted)
                    continue;
                blobItems.Add(new UserBlobItem { FileName = blobItem.Name, filePath = blobItem.Name });
            }

            if (blobItems.Count() == 0)
                throw new BlobClientException("all deleted", "BlobItem");

            return blobItems;
        }

        public void downdloadBlobItem(string containerName, string blobName)
        {
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            if (blobContainerClient == null)
                throw new BlobClientException("no data", $"BlobContainerClient({containerName})");

            List<UserBlobItem> userBlobItems = getBlobItemNames(containerName).ToList();
            var userBlobItem = userBlobItems.FirstOrDefault(ub => ub.fileName.Equals(blobName.ToLower())) ?? null;
            if (userBlobItem == null)
                throw new BlobClientException("no data", $"BlobItem({blobName})");

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            BlobClient blobClient = blobContainerClient.GetBlobClient(userBlobItem.filePath);
            BlobDownloadResult downloadResult = blobClient.DownloadContent();

            string downdloadPath = relativePath + blobName.ToLower();
            using (FileStream fileStream = File.OpenWrite(downdloadPath))
            {
                downloadResult.Content.ToStream().CopyTo(fileStream);
            }
        }
    }
}

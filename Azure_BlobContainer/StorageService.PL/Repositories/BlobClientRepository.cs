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

        public BlobClientRepository(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
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

        public IEnumerable<UserBlobItem> getBlobItems(string containerName)
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
    }
}

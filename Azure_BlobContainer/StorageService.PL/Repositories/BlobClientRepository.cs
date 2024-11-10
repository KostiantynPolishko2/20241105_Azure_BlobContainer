using Azure.Storage.Blobs;
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

        public string getStorageName()
        {
            if (this.blobServiceClient == null)
                throw new DirectoryNotFoundException("BlobServiceClient is null");

            return this.blobServiceClient.AccountName;
        }
    }
}

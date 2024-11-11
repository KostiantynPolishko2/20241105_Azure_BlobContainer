using StorageService.PL.Entities;

namespace StorageService.PL.Interfaces
{
    public interface IBlobClientRepository
    {
        public string getStorageName();

        public IEnumerable<string> getContainerNames();

        public IEnumerable<UserBlobItem> getBlobItemNames(string containerName);

        public void downdloadBlobItem(string containerName, string blobName);
    }
}

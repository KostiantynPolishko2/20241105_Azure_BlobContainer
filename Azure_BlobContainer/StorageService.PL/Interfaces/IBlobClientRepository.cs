using StorageService.PL.Entities;

namespace StorageService.PL.Interfaces
{
    public interface IBlobClientRepository
    {
        public string getStorageName();

        public IEnumerable<string> getContainerNames();

        public IEnumerable<UserBlobItem> getBlobItems(string containerName);
    }
}

namespace StorageService.PL.Infrastructures
{
    public class BlobClientException : Exception
    {
        public string property { get; } = null!;
        public BlobClientException(string message, string property) : base(message) {
            this.property = property;
        }
    }
}
